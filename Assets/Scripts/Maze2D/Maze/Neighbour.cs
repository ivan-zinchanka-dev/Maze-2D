using UnityEngine;

namespace Maze2D.Maze
{
    internal struct Neighbour {

        public Vector2Int Position;
        public WallState SharedWall;
    }
}