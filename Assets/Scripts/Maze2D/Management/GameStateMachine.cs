using System;
using System.Collections.Generic;
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
        
        private IDisposable _stateChangeListener;
        private readonly ICollection<IDisposable> _playerInputListeners = new CompositeDisposable();
        
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
            _stateChangeListener = _currentState.Pairwise().Subscribe(pair =>
            {
                if (pair.Previous != pair.Current)
                {
                    OnStateChanged(pair.Current);
                }
            });
        }

        private void OnEnable()
        {
            Observable.EveryUpdate()
                .Where(IsPauseDemand)
                .Subscribe(unit => _currentState.Value = GameState.Paused)
                .AddTo(_playerInputListeners);
        }

        private bool IsPauseDemand(long unit)
        {
            return _currentState.Value != GameState.Pending && 
                   _inputSystemService.GetButtonDown(InputActions.Pause);
        }
        
        private void OnStateChanged(GameState currentState)
        {
            switch (currentState)
            {
                case GameState.Played:
                    _playerController.enabled = true;
                    break;
                
                case GameState.Paused:
                    _playerController.enabled = false;
                    break;
            }
            
            _logger.LogDebug($"Game state: {currentState}");
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
        
        private void OnDisable()
        {
            _playerInputListeners.Clear();
        }

        private void OnDestroy()
        {
            _stateChangeListener?.Dispose();
        }
    }
}