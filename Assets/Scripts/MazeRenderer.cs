using UnityEngine;

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

    public int Width { get { return _width; } set { _width = value; } }
    public int Height { get { return _height; } set { _height = value; } }


    private void Draw(WallState[,] maze)
    {
        for (int i = 0; i < _width; i++) {

            for (int j = 0; j < _height; j++) {

                WallState cell = maze[i, j];
                Vector2 position = new Vector2(-_width / 2 + i, -_height / 2 + j);

                if (cell.HasFlag(WallState.RIGHT))
                {
                    Transform rightWall = Instantiate(_wallPrefab, transform) as Transform;
                    rightWall.position = position + new Vector2(0.0f, _cellSize / 2);
                    rightWall.localScale = new Vector2(rightWall.localScale.x, _cellSize);
                }

                if (cell.HasFlag(WallState.DOWN))
                {
                    Transform bottomWall = Instantiate(_wallPrefab, transform) as Transform;
                    bottomWall.position = position + new Vector2(-_cellSize / 2, 0.0f);
                    bottomWall.eulerAngles = new Vector3(0, 0, 90);
                    bottomWall.localScale = new Vector2(bottomWall.localScale.x, _cellSize);
                }

                if (i == 0)         // i == width - 1
                {                  
                    if (cell.HasFlag(WallState.LEFT))
                    {
                        Transform leftWall = Instantiate(_wallPrefab, transform) as Transform;
                        leftWall.position = position + new Vector2(-_cellSize, _cellSize / 2);           // ( - width)
                        leftWall.localScale = new Vector2(leftWall.localScale.x, _cellSize);
                    }
                }

                if (j == _height - 1) {           // j == 0

                    if (cell.HasFlag(WallState.UP))
                    {
                        Transform topWall = Instantiate(_wallPrefab, transform) as Transform;
                        topWall.position = position + new Vector2(-_cellSize / 2, CellSize);   // (-_cellSize / 2, height)
                        topWall.eulerAngles = new Vector3(0, 0, 90);
                        topWall.localScale = new Vector2(topWall.localScale.x, _cellSize);
                    }
                }
            }       
        }
 
    }
}
