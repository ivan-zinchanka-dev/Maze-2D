using System;
using DG.Tweening;
using Maze2D.Game;
using Maze2D.Maze;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Maze2D.Player
{
    [DisallowMultipleComponent]
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] 
        private float _speed = 3.0f;
        
        [SerializeField] 
        private SpriteRenderer _spriteRenderer;

        [Inject] 
        private StorageService _storageService;
        
        private PlayerMap _map;
        private Vector2Int _currentPosInMap;
        private Tween _movingTween;

        private void Reset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            _spriteRenderer.color = _storageService.GetPlayerColor();
        }

        public void SetMap(PlayerMap map)
        {
            _map = map;
            SetToMapCenter();
        }

        public void SetToMapCenter() {
            
            transform.position = GetStartPositionInWorldCoords(_map.CellSize);
            _currentPosInMap = WorldToMapCoords(transform.position);
        }
        
        public bool StepRight()
        {
            if (CanMoveTo(WallState.Right))
            {
                Step(new Vector2(transform.position.x + _map.CellSize, transform.position.y));
                _currentPosInMap.x++;
                return true;
            }

            return false;
        }
        
        public bool StepLeft()
        {
            if (CanMoveTo(WallState.Left))
            {
                Step(new Vector2(transform.position.x - _map.CellSize, transform.position.y));
                _currentPosInMap.x--;
                return true;
            }

            return false;
        }
        
        public bool StepUp()
        {
            if (CanMoveTo(WallState.Up))
            {
                Step(new Vector2(transform.position.x, transform.position.y + _map.CellSize));
                _currentPosInMap.y++;
                return true;
            }

            return false;
        }
        
        public bool StepDown()
        {
            if (CanMoveTo(WallState.Down))
            {
                Step(new Vector2(transform.position.x, transform.position.y - _map.CellSize));
                _currentPosInMap.y--;
                return true;
            }

            return false;
        }

        public bool IsFinished()
        {
            return _currentPosInMap.x < 0 || _currentPosInMap.x >= _map.Width ||
                   _currentPosInMap.y < 0 || _currentPosInMap.y >= _map.Height;
        }

        private Vector2 GetStartPositionInWorldCoords(float cellSize)
        {
            Vector2Int mask = new Vector2Int(Random.Range(0, 2), Random.Range(0, 2));
            if (mask.x == 0) mask.x = -1;
            if (mask.y == 0) mask.y = -1;

            return new Vector2(mask.x * cellSize / 2, mask.y * cellSize / 2);
        }

        private void Step(Vector2 target)
        {
            _movingTween = Step(target, _speed);
        }

        private Tween Step(Vector2 target, float speed)
        {
            return transform
                .DOMove(target, speed)
                .SetSpeedBased()
                .SetEase(Ease.Linear)
                .SetLink(gameObject);
        }

        private bool CanMoveTo(WallState direction)
        {
            return !_movingTween.IsActive() && !_map.Maze[_currentPosInMap.x, _currentPosInMap.y].HasFlag(direction);
        }
        
        private Vector2Int WorldToMapCoords(Vector2 source) {

            Vector2Int mapCoords = new Vector2Int(_map.Width / 2, _map.Height / 2);

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
    }
}