using Maze2D.Audio;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.View;
using Maze2D.Configs;
using Maze2D.Game;
using Maze2D.Maze;
using Maze2D.Player;
using Maze2D.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Maze2D.Management
{
    public class GameSceneScope : LifetimeScope
    {
        [SerializeField] 
        private HeadUpDisplay _headUpDisplay;
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
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IInputSystemService>(new InputSystemService(true));
            builder.RegisterInstance<StorageService>(new StorageService(Difficulty.Easy));
            builder.RegisterInstance<ViewFactory>(_viewFactory);
            builder.RegisterInstance<DifficultyConfigContainer>(_difficultyConfigContainer);
            builder.RegisterInstance<AudioManager>(_audioManager);

            builder.RegisterComponent<HeadUpDisplay>(_headUpDisplay);
            builder.RegisterComponent<GameStateMachine>(_gameStateMachine);
            builder.RegisterComponent<MazeRenderer>(_mazeRenderer);
            builder.RegisterComponent<PlayerControllerFactory>(_playerControllerFactory);
            
            
            builder.Register<MazeGenerator>(Lifetime.Singleton);
            builder.Register<PlayerMapGenerator>(Lifetime.Singleton);
        }
    }
}