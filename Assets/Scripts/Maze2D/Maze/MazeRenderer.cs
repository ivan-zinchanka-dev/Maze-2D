using System.Collections.Generic;
using Maze2D.Player;
using UnityEngine;

namespace Maze2D.Maze
{
    public class MazeRenderer : MonoBehaviour
    {
        [SerializeField] 
        private Transform _wallPrefab = null;
        
        [field: SerializeField]
        public float CellSize { get; private set; } = 1.0f;

        private readonly Stack<Transform> _walls = new Stack<Transform>();
        
        public PlayerMap RenderMaze(WallState[,] maze)
        {
            int width = maze.GetLength(0);
            int height = maze.GetLength(1);
            
            for (int i = 0; i < width; i++) {

                for (int j = 0; j < height; j++) {

                    WallState cell = maze[i, j];
                    Vector2 position = new Vector2(-width / 2.0f + i, -height / 2.0f + j);

                    if (cell.HasFlag(WallState.Right))
                    {
                        RenderWall(position + new Vector2(0.0f, CellSize / 2), Vector2.zero);
                    }

                    if (cell.HasFlag(WallState.Down))
                    {
                        RenderWall(position + new Vector2(-CellSize / 2, 0.0f), new Vector3(0, 0, 90));
                    }

                    if (i == 0)
                    {                  
                        if (cell.HasFlag(WallState.Left))
                        {
                            RenderWall(position + new Vector2(-CellSize, CellSize / 2), Vector2.zero);
                        }
                    }

                    if (j == height - 1) 
                    {
                        if (cell.HasFlag(WallState.Up))
                        {
                            RenderWall(position + new Vector2(-CellSize / 2, CellSize), new Vector3(0, 0, 90));
                        }
                    }
                }       
            }

            return new PlayerMap(maze, CellSize);
        }

        private void RenderWall(Vector2 position, Vector3 eulerAngles)
        {
            Transform wall = Instantiate(_wallPrefab, transform);
            wall.position = position;
            wall.eulerAngles = eulerAngles;
            wall.localScale = new Vector2(wall.localScale.x, CellSize);;
            _walls.Push(wall);
        }
    }
}