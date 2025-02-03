using System;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.View;
using Maze2D.Configs;
using Maze2D.Controls;
using Maze2D.Game;
using Maze2D.Maze;
using Maze2D.Player;
using Maze2D.UI;
using UnityEngine;
using VContainer;

namespace Maze2D.Management
{
    public class GameStateMachine : MonoBehaviour
    {
        [Inject] 
        private IInputSystemService _inputSystemService;
        [Inject] 
        private StorageService _storageService;
        [Inject] 
        private DifficultyConfigContainer _difficultyConfigContainer;
        [Inject] 
        private MazeGenerator _mazeGenerator;
        [Inject] 
        private MazeRenderer _mazeRenderer;
        [Inject] 
        private PlayerControllerFactory _playerFactory;
        [Inject] 
        private ViewFactory _viewFactory;
        
        private PlayerController _playerController;
        private GameState _currentState;
        private PauseMenu _pauseMenu;
        
        public GameState CurrentState
        {
            get => _currentState;

            set
            {
                GameState newState = value;
                
                if (_currentState != newState)
                {
                    _currentState = newState;
                    OnStateChanged();
                }
            }
        }

        private void OnStateChanged()
        {
            _playerController.enabled = _currentState == GameState.Played;

            switch (_currentState)
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
        }

        private void OnPauseMenuCommandInvoked(PauseMenu.CommandKind command)
        {
            switch (command)
            {
                case PauseMenu.CommandKind.RestartLevel:
                    CurrentState = GameState.Played;
                    break;
                case PauseMenu.CommandKind.RegenerateLevel:
                    CurrentState = GameState.Played;
                    break;
                case PauseMenu.CommandKind.ToMainMenu:
                    break;
                case PauseMenu.CommandKind.Continue:
                    CurrentState = GameState.Played;
                    break;
            }
        }

        private void Awake()
        {
            Difficulty difficultyLevel = _storageService.GetDifficulty();
            
            DifficultyConfig config = _difficultyConfigContainer.GetConfigByLevel(difficultyLevel);
            WallState[,] maze = _mazeGenerator.Generate(config.MazeWidth, config.MazeHeight);
            PlayerMap map = _mazeRenderer.Draw(maze);

            _playerController = _playerFactory.CreatePlayer();
            _playerController.View.SetMap(map);
            _playerController.Finished.AddListener(OnMapFinished);
            
            CurrentState = GameState.Played;
        }
        
        private void OnMapFinished()
        {
            Debug.Log("Map finished");
        }

        private void Update()
        {
            if (_inputSystemService.GetButtonDown(InputActions.Pause))
            {
                CurrentState = GameState.Paused;
            }
        }
        
    }
}