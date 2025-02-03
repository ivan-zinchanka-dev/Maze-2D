using Maze2D.CodeBase.Controls;
using Maze2D.Configs;
using Maze2D.Game;
using Maze2D.Maze;
using Maze2D.Player;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Maze2D.Management
{
    public class GameSceneScope : LifetimeScope
    {
        [SerializeField] private GameStateMachine _gameStateMachine;
        [SerializeField] private MazeRenderer _mazeRenderer;
        [SerializeField] private PlayerControllerFactory _playerControllerFactory;
        
        [SerializeField] private DifficultyConfigContainer _difficultyConfigContainer;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IInputSystemService>(new InputSystemService(true));
            builder.RegisterInstance<StorageService>(new StorageService(Difficulty. Normal));
            builder.RegisterInstance<DifficultyConfigContainer>(_difficultyConfigContainer);

            builder.RegisterComponent<GameStateMachine>(_gameStateMachine);
            builder.Register<MazeGenerator>(Lifetime.Singleton);
            builder.RegisterComponent<MazeRenderer>(_mazeRenderer);
            builder.RegisterComponent<PlayerControllerFactory>(_playerControllerFactory);
        }
    }
}