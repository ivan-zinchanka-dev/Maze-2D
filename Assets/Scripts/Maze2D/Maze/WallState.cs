using System;

namespace Maze2D.Maze
{
    [Flags]
    internal enum WallState
    {
        // 0000 - NO WALLS
        // 1111 - LEFT, RIGHT, UP, DOWN
        Right = 2, // 0010
        Left = 1, // 0001    
        Down = 8, // 1000  
        Up = 4,  // 0100
      
        Visited = 128
    }
}