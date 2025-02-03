using System;
using Maze2D.CodeBase.Controls;
using Maze2D.Configs;
using Maze2D.Controls;
using Maze2D.Game;
using Maze2D.Maze;
using Maze2D.Player;
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
        private PlayerController _playerController;
        [Inject] 
        private PlayerView _playerView;
        
        
        private GameState _currentState;
        
        public GameState CurrentState
        {
            get => _currentState;

            set
            {
                _currentState = value;
                _playerController.enabled = _currentState == GameState.Played;
            }
        }

        private void Awake()
        {
            Difficulty difficultyLevel = _storageService.GetDifficulty();
            
            DifficultyConfig config = _difficultyConfigContainer.GetConfigByLevel(difficultyLevel);
            WallState[,] maze = _mazeGenerator.Generate(config.MazeWidth, config.MazeHeight);
            PlayerMap map = _mazeRenderer.Draw(maze);
            _playerView.SetMap(map);
            
            CurrentState = GameState.Played;
        }

        private void OnEnable()
        {
            _playerController.Finished += OnMapFinished;
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
        
        private void OnDisable()
        {
            _playerController.Finished -= OnMapFinished;
        }
    }
}