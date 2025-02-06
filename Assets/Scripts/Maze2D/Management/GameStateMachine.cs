using System;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.View;
using Maze2D.Controls;
using Maze2D.Maze;
using Maze2D.Player;
using Maze2D.UI;
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
        private PauseMenu _pauseMenu;

        private IDisposable _stateChanging;
        
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;

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
            
            if (_pauseMenu != null)
            {
                Destroy(_pauseMenu.gameObject);
            }
        }
        
        private void OnPaused()
        {
            _playerController.enabled = false;
            _pauseMenu = _viewFactory.CreateView<PauseMenu>();
            _pauseMenu.CommandInvoked.AddListener(OnPauseMenuCommandInvoked);
            
            // TODO Paused event or ReactiveCommand for HUD
        }

        private async void OnPauseMenuCommandInvoked(PauseMenu.CommandKind command)
        {
            switch (command)
            {
                case PauseMenu.CommandKind.RestartLevel:
                    _playerController.View.SetToMapCenter();
                    _currentState.Value = GameState.Played;
                    break;
                
                case PauseMenu.CommandKind.RegenerateLevel:
                    PlayerMap map = await _mapGenerator.GeneratePlayerMap();
                    _playerController.View.SetMap(map);
                    _currentState.Value = GameState.Played;
                    break;
                
                case PauseMenu.CommandKind.ToMainMenu:
                    break;
                
                case PauseMenu.CommandKind.Continue:
                    _currentState.Value = GameState.Played;
                    break;
            }
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