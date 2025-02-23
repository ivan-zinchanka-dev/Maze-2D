using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Maze2D.Maze;

namespace Maze2D.Player
{
    internal readonly struct PlayerMap : IAsyncDisposable
    {
        private readonly Func<UniTask> _disposeMethod;
        
        public WallState[,] Maze { get; }
        public float CellSize { get; }

        public int Width => Maze.GetLength(0); 
        public int Height => Maze.GetLength(1); 
        
        public PlayerMap(WallState[,] maze, float cellSize, Func<UniTask> disposeMethod)
        {
            Maze = maze;
            CellSize = cellSize;
            _disposeMethod = disposeMethod;
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_disposeMethod != null)
            {
                await _disposeMethod.Invoke();
            }
        }
    }
}