﻿using System;
using Cysharp.Threading.Tasks;
using Maze2D.CodeBase.Logging.Contracts;
using Maze2D.CodeBase.Logging.Contracts.Generic;
using Maze2D.CodeBase.Logging.Extensions;
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
        private readonly ILogger<PlayerMapGenerator> _logger;
        
        public PlayerMapGenerator(
            Lazy<Settings> settings, 
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