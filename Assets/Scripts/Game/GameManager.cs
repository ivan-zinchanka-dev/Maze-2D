using Maze;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } = null;
        public bool Pause { get; private set; } = false;

        [SerializeField] private PlayerController _playerPrefab = null;
        [SerializeField] private MazeRenderer _mazeRenderer = null;
        [SerializeField] private RectTransform _gameMenu = null;

        private Vector2 _playerStartPosition = default;
        private PlayerController _player = null;
        
        public void ToMainMenu()
        {
            SceneManager.LoadScene(ScenesUtility.StartSceneIndex);
        }

        public void RegenerateMaze()
        {
            SceneManager.LoadScene(ScenesUtility.GameSceneIndex);
        }

        public void RestartPlayerProgress()
        {
            _player.SetPosition(_playerStartPosition);
            _gameMenu.gameObject.SetActive(false);
            Pause = false;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            switch (StorageUtility.GetDifficulty()) {

                case Difficulty.Easy:
                    _mazeRenderer.Width = 20;
                    _mazeRenderer.Height = 20;
                    break;

                case Difficulty.Normal:
                default:
                    _mazeRenderer.Width = 30;
                    _mazeRenderer.Height = 30;
                    break;
                
                case Difficulty.Hard:
                    _mazeRenderer.Width = 40;
                    _mazeRenderer.Height = 30;
                    break;
            }

      
            Vector2Int mask = new Vector2Int(Random.Range(0, 2), Random.Range(0, 2));
            if (mask.x == 0) mask.x = -1;
            if (mask.y == 0) mask.y = -1;

            _playerStartPosition = new Vector2(mask.x * _mazeRenderer.CellSize / 2, mask.y * _mazeRenderer.CellSize / 2);

            _player = Instantiate(_playerPrefab, _playerStartPosition, Quaternion.identity);
            _player.Initialize(_mazeRenderer.Maze, _mazeRenderer.CellSize, _playerStartPosition);

            _player.SpriteRenderer.color = StorageUtility.GetPlayerColor();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Pause")) {
         
                Pause = !Pause;
                _gameMenu.gameObject.SetActive(Pause);                  
            }
        }

    }
}