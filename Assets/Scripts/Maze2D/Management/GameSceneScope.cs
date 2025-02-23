using System;
using JanZinch.Services.Audio.Contracts;
using JanZinch.Services.InputSystem.Contracts;
using JanZinch.Services.InputSystem.Standard;
using JanZinch.Services.Logging;
using JanZinch.Services.Logging.Contracts;
using JanZinch.Services.Logging.Standard;
using Maze2D.Audio;
using Maze2D.Configs;
using Maze2D.Maze;
using Maze2D.Player;
using Maze2D.Settings;
using Maze2D.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Maze2D.Management
{
    public class GameSceneScope : LifetimeScope
    { 
        [Header("References")]
        [SerializeField] 
        private GameNavigationManager _gameNavigationManager;
        [SerializeField] 
        private GameStateMachine _gameStateMachine;
        [SerializeField] 
        private MazeRenderer _mazeRenderer;
        [SerializeField] 
        private PlayerControllerFactory _playerControllerFactory;
        [SerializeField] 
        private AudioManager _audioManager;
        [SerializeField] 
        private ViewFactory _viewFactory;
        [SerializeField] 
        private DifficultyConfigContainer _difficultyConfigContainer;
        [SerializeField] 
        private DefaultSettingsConfig _defaultSettingsConfig;
        
        [Header("Configuration")]
        [SerializeField] 
        private LogEventLevel _minimumLogLevel = LogEventLevel.Debug;
        
        protected override void Configure(IContainerBuilder builder)
        {
            StorageService storageService = new StorageService(_defaultSettingsConfig.Settings);

            builder.RegisterInstance<ILoggerFactory>(new UnityLoggerFactory(_minimumLogLevel));
            
            builder.RegisterInstance<StorageService>(storageService);
            builder.RegisterInstance<Lazy<Settings.Settings>>(storageService.Settings);
            builder.RegisterInstance<IInputSystemService>(new InputSystemService(true));
            
            builder.RegisterInstance<ViewFactory>(_viewFactory);
            builder.RegisterInstance<DifficultyConfigContainer>(_difficultyConfigContainer);
            builder.RegisterInstance<IAudioManger>(_audioManager);

            builder.RegisterComponent<GameNavigationManager>(_gameNavigationManager);
            builder.RegisterComponent<GameStateMachine>(_gameStateMachine);
            builder.RegisterComponent<MazeRenderer>(_mazeRenderer);
            builder.RegisterComponent<PlayerControllerFactory>(_playerControllerFactory);
            
            builder.Register<MazeGenerator>(Lifetime.Singleton);
            builder.Register<PlayerMapGenerator>(Lifetime.Singleton);
        }
    }
}