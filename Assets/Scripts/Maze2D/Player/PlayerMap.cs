using Maze2D.Maze;

namespace Maze2D.Player
{
    public readonly struct PlayerMap
    {
        public WallState[,] Maze { get; }
        public float CellSize { get; }

        public int Width => Maze.GetLength(0); 
        public int Height => Maze.GetLength(1); 
        
        public PlayerMap(WallState[,] maze, float cellSize)
        {
            Maze = maze;
            CellSize = cellSize;
        }
    }
}