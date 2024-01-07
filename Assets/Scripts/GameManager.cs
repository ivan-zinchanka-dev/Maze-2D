using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;

    public static Difficulty DifficultyLevel { get; set; } = Difficulty.NORMAL;
    public static PlayerCustomisations PlayerCustoms { get; set; } = new PlayerCustomisations() { RGBAColor = Color.white, ColorIndex = 0 };

    public bool Pause { get; set; } = false;

    [SerializeField] private PlayerController _playerPrefab = null;
    [SerializeField] private MazeRenderer _mazeRenderer = null;
    [SerializeField] private RectTransform _gameMenu = null;

    private Vector2 _playerStartPosition = default;
    private PlayerController _player = null;
    

    private void Awake()
    {
        Instance = this;
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(GUIManager.MainMenuScene);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(GUIManager.GameSessionScene);
    }

    public void ResetPlayer()
    {
        _player.ReInitialize(_playerStartPosition);
        _gameMenu.gameObject.SetActive(false);
        Pause = false;
    }


    private void Start()
    {
        switch (DifficultyLevel) {

            case Difficulty.EASY:

                _mazeRenderer.Width = 20;
                _mazeRenderer.Height = 20;

                    break;

            case Difficulty.NORMAL:
            default:

                _mazeRenderer.Width = 30;
                _mazeRenderer.Height = 30;

                break;


            case Difficulty.HARD:

                _mazeRenderer.Width = 40;
                _mazeRenderer.Height = 30;

                break;
        }

      
        Vector2Int mask = new Vector2Int(Random.Range(0, 2), Random.Range(0, 2));
        if (mask.x == 0) mask.x = -1;
        if (mask.y == 0) mask.y = -1;

        _playerStartPosition = new Vector2(mask.x * _mazeRenderer.CellSize / 2, mask.y * _mazeRenderer.CellSize / 2);

        _player = Instantiate(_playerPrefab, _playerStartPosition, Quaternion.identity) as PlayerController;
        _player.Initialize(_mazeRenderer.Maze, _mazeRenderer.CellSize, _playerStartPosition);

        _player.Sprite.color = PlayerCustoms.RGBAColor;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause")) {
         
            Pause = !Pause;
            _gameMenu.gameObject.SetActive(Pause);                  
        }
    }

}

public struct PlayerCustomisations
{
    public static PlayerCustomisations Default { get; } = new PlayerCustomisations { ColorIndex = 0, RGBAColor = Color.white };

    public Color RGBAColor { get; set; }
    public int ColorIndex { get; set; }

}


public enum Difficulty : byte { 

    EASY, NORMAL, HARD

}