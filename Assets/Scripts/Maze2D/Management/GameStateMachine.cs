using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.Logging.Contracts;
using Maze2D.CodeBase.Logging.Contracts.Generic;
using Maze2D.CodeBase.Logging.Extensions;
using Maze2D.CodeBase.View;
using Maze2D.Controls;
using Maze2D.Maze;
using Maze2D.Player;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Maze2D.Management
{
    public class GameStateMachine : MonoBehaviour
    {
        [Inject] 
        private IInputSystemService _inputSystemService;
        [Inject] 
        private PlayerControllerFactory _playerFactory;
        [Inject] 
        private ViewFactory _viewFactory;
        [Inject] 
        private PlayerMapGenerator _mapGenerator;
        
        private PlayerController _playerController;
        private readonly ReactiveProperty<GameState> _currentState = new (GameState.Pending);
        private IDisposable _stateChanging;
        private ILogger<GameStateMachine> _logger;
        
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;
        public UnityEvent LevelFinished => _playerController.Finished;
        
        [Inject]
        private void InjectLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GameStateMachine>();
        }
        
        public async UniTask PlayAsync(Action onMapFinished)
        {
            PlayerMap map = await _mapGenerator.GeneratePlayerMapAsync();

            _playerController = _playerFactory.CreatePlayer();
            _playerController.View.Map = map;
            _playerController.Finished.AddListener(OnMapFinished);
            _playerController.Finished.AddListener(()=> onMapFinished?.Invoke());
            await _playerController.View.ShowAsync();
            
            _currentState.Value = GameState.Played;
        }

        private void Awake()
        {
            _stateChanging = _currentState.Pairwise().Subscribe(pair =>
            {
                if (pair.Previous != pair.Current)
                {
                    OnStateChanged(pair.Current);
                }
            });
        }

        private void OnStateChanged(GameState currentState)
        {
            switch (currentState)
            {
                case GameState.Played:
                    OnPlayed();
                    break;
                
                case GameState.Paused:
                    OnPaused();
                    break;
            }
            
            _logger.LogDebug($"Game state: {currentState}");
        }

        private void OnPlayed()
        {
            _playerController.enabled = true;
        }
        
        private void OnPaused()
        {
            _playerController.enabled = false;
        }

        public async UniTask HidePlayerAsync()
        {
            await _playerController.View.HideAsync();
        }
        
        public void RestartLevel()
        {
            _playerController.View.SetToMapCenter();
            _currentState.Value = GameState.Played;
        }
        
        public async UniTask ShowPlayerAsync()
        {
            await _playerController.View.ShowAsync();
        }
        
        public void Continue()
        {
            _currentState.Value = GameState.Played;
        }

        public async UniTask ExitAsync()
        {
            await _playerController.View.HideAsync();
            await _playerController.View.Map.DisposeAsync();
            _playerController.Finished.RemoveAllListeners();
            Destroy(_playerController.gameObject);
        }

        private void OnMapFinished()
        {
            _playerController.enabled = false;
            _currentState.Value = GameState.Pending;
        }

        private void Update()
        {
            if (_currentState.Value != GameState.Pending && _inputSystemService.GetButtonDown(InputActions.Pause))
            {
                _currentState.Value = GameState.Paused;
            }
        }

        private void OnDestroy()
        {
            _stateChanging?.Dispose();
            
            /*if (CurrentState.Value == GameState.Played || CurrentState.Value == GameState.Paused)
            {
                _playerController.Finished.RemoveListener(OnMapFinished);
            }*/
        }
    }
}