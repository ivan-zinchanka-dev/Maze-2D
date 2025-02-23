using System;
using Maze2D.Settings;

namespace Maze2D.Configs
{
    [Serializable]
    internal struct DifficultyConfig
    {
        public Difficulty Level;
        public int MazeWidth;
        public int MazeHeight;
    }
}