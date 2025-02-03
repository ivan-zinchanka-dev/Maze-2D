using Maze2D.Configs;
using Maze2D.Game;
using Maze2D.Player;
using UnityEngine;
using VContainer;

namespace Maze2D.Maze
{
    public class MazeRenderer : MonoBehaviour
    {
        /*[field: SerializeField, Range(10, 50)] 
        public int Width { get; private set; } = 20;
        
        [field: SerializeField, Range(10, 50)] 
        public int Height { get; private set; } = 10;*/

        [SerializeField] 
        private Transform _wallPrefab = null;
        
        [field: SerializeField]
        public float CellSize { get; private set; } = 1.0f;
        
        public PlayerMap Draw(WallState[,] maze)
        {
            int width = maze.GetLength(0);
            int height = maze.GetLength(1);
            
            for (int i = 0; i < width; i++) {

                for (int j = 0; j < height; j++) {

                    WallState cell = maze[i, j];
                    Vector2 position = new Vector2(-width / 2.0f + i, -height / 2.0f + j);

                    if (cell.HasFlag(WallState.Right))
                    {
                        Transform rightWall = Instantiate(_wallPrefab, transform);
                        rightWall.position = position + new Vector2(0.0f, CellSize / 2);
                        rightWall.localScale = new Vector2(rightWall.localScale.x, CellSize);
                    }

                    if (cell.HasFlag(WallState.Down))
                    {
                        Transform bottomWall = Instantiate(_wallPrefab, transform);
                        bottomWall.position = position + new Vector2(-CellSize / 2, 0.0f);
                        bottomWall.eulerAngles = new Vector3(0, 0, 90);
                        bottomWall.localScale = new Vector2(bottomWall.localScale.x, CellSize);
                    }

                    if (i == 0)
                    {                  
                        if (cell.HasFlag(WallState.Left))
                        {
                            Transform leftWall = Instantiate(_wallPrefab, transform);
                            leftWall.position = position + new Vector2(-CellSize, CellSize / 2);
                            leftWall.localScale = new Vector2(leftWall.localScale.x, CellSize);
                        }
                    }

                    if (j == height - 1) 
                    {
                        if (cell.HasFlag(WallState.Up))
                        {
                            Transform topWall = Instantiate(_wallPrefab, transform);
                            topWall.position = position + new Vector2(-CellSize / 2, CellSize);
                            topWall.eulerAngles = new Vector3(0, 0, 90);
                            topWall.localScale = new Vector2(topWall.localScale.x, CellSize);
                        }
                    }
                }       
            }

            return new PlayerMap(maze, CellSize);
        }
    }
}