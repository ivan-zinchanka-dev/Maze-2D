using System;
using Cysharp.Threading.Tasks;
using Maze2D.Configs;
using Maze2D.Domain;
using Maze2D.Player;

namespace Maze2D.Maze
{
    public class PlayerMapGenerator
    {
        private readonly Lazy<Settings> _settings;
        private readonly DifficultyConfigContainer _difficultyConfigContainer;
        private readonly MazeGenerator _mazeGenerator;
        private readonly MazeRenderer _mazeRenderer;

        public PlayerMapGenerator(
            Lazy<Settings> settings, 
            DifficultyConfigContainer difficultyConfigContainer, 
            MazeGenerator mazeGenerator, 
            MazeRenderer mazeRenderer)
        {
            _settings = settings;
            _difficultyConfigContainer = difficultyConfigContainer;
            _mazeGenerator = mazeGenerator;
            _mazeRenderer = mazeRenderer;
        }

        public async UniTask<PlayerMap> GeneratePlayerMapAsync()
        {
            Difficulty difficultyLevel = _settings.Value.GameDifficulty.Value;
            DifficultyConfig config = _difficultyConfigContainer.GetConfigByLevel(difficultyLevel);
            
            WallState[,] maze = _mazeGenerator.Generate(config.MazeWidth, config.MazeHeight);
            return await _mazeRenderer.RenderMazeAsync(maze);
        }
    }
}