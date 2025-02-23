using System;
using Cysharp.Threading.Tasks;
using JanZinch.Services.Logging.Contracts;
using JanZinch.Services.Logging.Contracts.Generic;
using JanZinch.Services.Logging.Extensions;
using Maze2D.Configs;
using Maze2D.Maze;
using Maze2D.Player;
using Maze2D.Settings;

namespace Maze2D.Management
{
    internal class PlayerMapGenerator
    {
        private readonly Lazy<Settings.Settings> _settings;
        private readonly DifficultyConfigContainer _difficultyConfigContainer;
        private readonly MazeGenerator _mazeGenerator;
        private readonly MazeRenderer _mazeRenderer;
        private readonly ILogger<PlayerMapGenerator> _logger;
        
        public PlayerMapGenerator(
            Lazy<Settings.Settings> settings, 
            DifficultyConfigContainer difficultyConfigContainer, 
            MazeGenerator mazeGenerator, 
            MazeRenderer mazeRenderer,
            ILoggerFactory loggerFactory)
        {
            _settings = settings;
            _difficultyConfigContainer = difficultyConfigContainer;
            _mazeGenerator = mazeGenerator;
            _mazeRenderer = mazeRenderer;
            _logger = loggerFactory.CreateLogger<PlayerMapGenerator>();
        }

        public async UniTask<PlayerMap> GeneratePlayerMapAsync()
        {
            _logger.LogVerbose("Start player map generation");
            
            Difficulty difficultyLevel = _settings.Value.GameDifficulty.Value;
            DifficultyConfig config = _difficultyConfigContainer.GetConfigByLevel(difficultyLevel);
            
            WallState[,] maze = _mazeGenerator.Generate(config.MazeWidth, config.MazeHeight);
            _logger.LogVerbose("Maze has been generated");
            
            PlayerMap playerMap = await _mazeRenderer.RenderMazeAsync(maze);
            _logger.LogVerbose("Maze animation completed");

            return playerMap;
        }
    }
}