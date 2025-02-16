using System;
using Maze2D.Domain;

namespace Maze2D.Configs
{
    [Serializable]
    public struct DifficultyConfig
    {
        public Difficulty Level;
        public int MazeWidth;
        public int MazeHeight;
    }
}