using UnityEngine;

namespace Maze
{
    public class MazeRenderer : MonoBehaviour
    {
        [SerializeField] [Range(10, 50)] private int _width = 10;
        [SerializeField] [Range(10, 50)] private int _height = 10;

        [SerializeField] private Transform _wallPrefab = null;

        [SerializeField] private float _cellSize = 1.0f;

        public float CellSize { get { return _cellSize; } }

        public WallState[,] Maze {

            get {

                var maze = MazeGenerator.Generate(_width, _height);
                Draw(maze);
                return maze;
            }    
        }

        public int Width { 
            get => _width;
            set => _width = value;
        }
        public int Height { 
            get => _height;
            set => _height = value;
        }


        private void Draw(WallState[,] maze)
        {
            for (int i = 0; i < _width; i++) {

                for (int j = 0; j < _height; j++) {

                    WallState cell = maze[i, j];
                    Vector2 position = new Vector2(-_width / 2.0f + i, -_height / 2.0f + j);

                    if (cell.HasFlag(WallState.Right))
                    {
                        Transform rightWall = Instantiate(_wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector2(0.0f, _cellSize / 2);
                        rightWall.localScale = new Vector2(rightWall.localScale.x, _cellSize);
                    }

                    if (cell.HasFlag(WallState.Down))
                    {
                        Transform bottomWall = Instantiate(_wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector2(-_cellSize / 2, 0.0f);
                        bottomWall.eulerAngles = new Vector3(0, 0, 90);
                        bottomWall.localScale = new Vector2(bottomWall.localScale.x, _cellSize);
                    }

                    if (i == 0)
                    {                  
                        if (cell.HasFlag(WallState.Left))
                        {
                            Transform leftWall = Instantiate(_wallPrefab, transform) as Transform;
                            leftWall.position = position + new Vector2(-_cellSize, _cellSize / 2);
                            leftWall.localScale = new Vector2(leftWall.localScale.x, _cellSize);
                        }
                    }

                    if (j == _height - 1) 
                    {
                        if (cell.HasFlag(WallState.Up))
                        {
                            Transform topWall = Instantiate(_wallPrefab, transform) as Transform;
                            topWall.position = position + new Vector2(-_cellSize / 2, CellSize);
                            topWall.eulerAngles = new Vector3(0, 0, 90);
                            topWall.localScale = new Vector2(topWall.localScale.x, _cellSize);
                        }
                    }
                }       
            }
 
        }
    }
}
