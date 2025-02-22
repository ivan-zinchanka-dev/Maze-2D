using System;
using Maze2D.Audio;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.Logging;
using Maze2D.CodeBase.Logging.Contracts;
using Maze2D.CodeBase.View;
using Maze2D.Configs;
using Maze2D.Domain;
using Maze2D.Maze;
using Maze2D.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Maze2D.Management
{
    public class GameSceneScope : LifetimeScope
    { 
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
        
        protected override void Configure(IContainerBuilder builder)
        {
            StorageService storageService = new StorageService(_defaultSettingsConfig.Settings);

            builder.Register<ILoggerFactory, UnityLoggerFactory>(Lifetime.Singleton);
            
            builder.RegisterInstance<StorageService>(storageService);
            builder.RegisterInstance<Lazy<Settings>>(storageService.Settings);
            builder.RegisterInstance<IInputSystemService>(new InputSystemService(true));
            
            builder.RegisterInstance<ViewFactory>(_viewFactory);
            builder.RegisterInstance<DifficultyConfigContainer>(_difficultyConfigContainer);
            builder.RegisterInstance<AudioManager>(_audioManager);

            builder.RegisterComponent<GameNavigationManager>(_gameNavigationManager);
            builder.RegisterComponent<GameStateMachine>(_gameStateMachine);
            builder.RegisterComponent<MazeRenderer>(_mazeRenderer);
            builder.RegisterComponent<PlayerControllerFactory>(_playerControllerFactory);
            
            builder.Register<MazeGenerator>(Lifetime.Singleton);
            builder.Register<PlayerMapGenerator>(Lifetime.Singleton);
        }
    }
}