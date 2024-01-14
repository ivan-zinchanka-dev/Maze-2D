using System.Collections;
using Maze;
using UnityEngine;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;

        private WallState[,] _map = null;
        private float _mapCellSize = default;
        private Vector2Int _currentPosInMap = default;
        private bool _canMove = true;
        
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private Vector2Int WorldToMapCoords(Vector2 source) {

            Vector2Int mapCoords = new Vector2Int(_map.GetLength(0) / 2, _map.GetLength(1) / 2);

            if (source.y < 0)
            {
                mapCoords += Vector2Int.down;
            }

            if (source.x > 0)
            {
                mapCoords += Vector2Int.right;
            }

            return mapCoords;
        }


        public void Initialize(WallState[,] map, float mapCellSize, Vector2 startPosition)
        {
            _map = map;
            _mapCellSize = mapCellSize;
            SetPosition(startPosition);
        }

        public void SetPosition(Vector2 startPosition) {

            _canMove = true;
            transform.position = startPosition;
            _currentPosInMap = WorldToMapCoords(startPosition);
        }

        private IEnumerator MakeStep(Vector2 target, float speed) {

            _canMove = false;
        
            while (transform.position.x != target.x || transform.position.y != target.y)
            {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }

            _canMove = true;
            yield return null;    
        }

        private void StartMakingStep(Vector3 target)
        {
            StartCoroutine(MakeStep(target, _speed));
        }

        private void Update()
        {
            if (!_canMove || GameManager.Instance.Pause) return;

            if (_currentPosInMap.x < 0 || _currentPosInMap.x >= _map.GetLength(0) ||
                _currentPosInMap.y < 0 || _currentPosInMap.y >= _map.GetLength(1))
            {
                GameManager.Instance.RegenerateMaze();
            }
            else if (Input.GetButton("Left") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Left))
            {
                StartMakingStep(new Vector2(transform.position.x - _mapCellSize, transform.position.y));
                _currentPosInMap.x--;
            }
            else if (Input.GetButton("Right") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Right))
            {
                StartMakingStep(new Vector2(transform.position.x + _mapCellSize, transform.position.y));
                _currentPosInMap.x++;
            }
            else if (Input.GetButton("Up") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Up))
            {
                StartMakingStep(new Vector2(transform.position.x, transform.position.y + _mapCellSize));
                _currentPosInMap.y++;
            }
            else if (Input.GetButton("Down") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Down))
            {
                StartMakingStep(new Vector2(transform.position.x, transform.position.y - _mapCellSize));
                _currentPosInMap.y--;
            }
            
        }

    }
}
