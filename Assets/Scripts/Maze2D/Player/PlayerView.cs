using DG.Tweening;
using Maze2D.Maze;
using UnityEngine;

namespace Maze2D.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private float _speed = 5.0f;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private WallState[,] _map;
        private float _mapCellSize;
        private Vector2Int _currentPosInMap;

        private Tween _movingTween;
        
        public void Initialize(WallState[,] map, float mapCellSize)
        {
            _map = map;
            _mapCellSize = mapCellSize;
            SetToMapCenter();
        }

        public void SetToMapCenter() {
            
            transform.position = GetStartPositionInWorldCoords(_mapCellSize);
            _currentPosInMap = WorldToMapCoords(transform.position);
        }
        
        public bool MakeStepRight()
        {
            if (CanMoveTo(WallState.Right))
            {
                MakeStep(new Vector2(transform.position.x + _mapCellSize, transform.position.y));
                _currentPosInMap.x++;
                return true;
            }

            return false;
        }
        
        public bool MakeStepLeft()
        {
            if (CanMoveTo(WallState.Left))
            {
                MakeStep(new Vector2(transform.position.x - _mapCellSize, transform.position.y));
                _currentPosInMap.x--;
                return true;
            }

            return false;
        }
        
        public bool MakeStepUp()
        {
            if (CanMoveTo(WallState.Up))
            {
                MakeStep(new Vector2(transform.position.x, transform.position.y + _mapCellSize));
                _currentPosInMap.y++;
                return true;
            }

            return false;
        }
        
        public bool MakeStepDown()
        {
            if (CanMoveTo(WallState.Down))
            {
                MakeStep(new Vector2(transform.position.x, transform.position.y - _mapCellSize));
                _currentPosInMap.y--;
                return true;
            }

            return false;
        }

        public bool IsFinished()
        {
            return _currentPosInMap.x < 0 || _currentPosInMap.x >= _map.GetLength(0) ||
                   _currentPosInMap.y < 0 || _currentPosInMap.y >= _map.GetLength(1);
        }

        private Vector2 GetStartPositionInWorldCoords(float cellSize)
        {
            Vector2Int mask = new Vector2Int(Random.Range(0, 2), Random.Range(0, 2));
            if (mask.x == 0) mask.x = -1;
            if (mask.y == 0) mask.y = -1;

            return new Vector2(mask.x * cellSize / 2, mask.y * cellSize / 2);
        }

        private void MakeStep(Vector2 target)
        {
            _movingTween = MakeStep(target, _speed);
        }

        private Tween MakeStep(Vector2 target, float speed)
        {
            return transform
                .DOMove(target, speed)
                .SetSpeedBased()
                .SetEase(Ease.OutSine)
                .SetLink(gameObject);
        }

        private bool CanMoveTo(WallState direction)
        {
            return !_movingTween.IsActive() && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(direction);
        }
        
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
    }
}