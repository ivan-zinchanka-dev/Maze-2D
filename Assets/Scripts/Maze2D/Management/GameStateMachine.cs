using System;
using Cysharp.Threading.Tasks;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.View;
using Maze2D.Controls;
using Maze2D.Maze;
using Maze2D.Player;
using UniRx;
using UnityEngine;
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
        
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;

        //public IReactiveCommand<>
        
        public async void Play()
        {
            PlayerMap map = await _mapGenerator.GeneratePlayerMap();

            _playerController = _playerFactory.CreatePlayer();
            _playerController.View.SetMap(map);
            _playerController.Finished.AddListener(OnMapFinished);
            
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
        }

        private void OnPlayed()
        {
            _playerController.enabled = true;
        }
        
        private void OnPaused()
        {
            _playerController.enabled = false;
        }


        public void RestartLevel()
        {
            _playerController.View.SetToMapCenter();
            _currentState.Value = GameState.Played;
        }
        
        public async UniTask RegenerateLevel()
        {
            PlayerMap map = await _mapGenerator.GeneratePlayerMap();
            _playerController.View.SetMap(map);
            _currentState.Value = GameState.Played;
        }

        public void Continue()
        {
            _currentState.Value = GameState.Played;
        }
        

        private async void OnMapFinished()
        {
            PlayerMap map = await _mapGenerator.GeneratePlayerMap();
            _playerController.View.SetMap(map);
            
            _currentState.Value = GameState.Pending;
        }

        private void Update()
        {
            if (_inputSystemService.GetButtonDown(InputActions.Pause))
            {
                _currentState.Value = GameState.Paused;
            }
        }

        private void OnDestroy()
        {
            _stateChanging?.Dispose();
            
            if (CurrentState.Value == GameState.Played || CurrentState.Value == GameState.Paused)
            {
                _playerController.Finished.RemoveListener(OnMapFinished);
            }
        }
    }
}