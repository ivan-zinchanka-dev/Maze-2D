using System;
using System.Collections;
using Maze;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private SpriteRenderer _spriteRender = null;

    private WallState[,] _map = null;
    private float _mapCellSize = default;
    private Vector2Int _currentPosInMap = default;
    private bool _canMove = true;

    
    public SpriteRenderer Renderer => _spriteRender;


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


    private void Update()
    {
        if (!_canMove || GameManager.Instance.Pause) return;

        try
        {

            if (_currentPosInMap.x >= _map.GetLength(0) || _currentPosInMap.y >= _map.GetLength(1))
            {
                GameManager.Instance.RestartGame();
            }
            else if (Input.GetButton("Left") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Left))
            {
                StartCoroutine(MakeStep(new Vector2(transform.position.x - _mapCellSize, transform.position.y), _speed));
                _currentPosInMap.x--;
            }
            else if (Input.GetButton("Right") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Right))
            {
                StartCoroutine(MakeStep(new Vector2(transform.position.x + _mapCellSize, transform.position.y), _speed));
                _currentPosInMap.x++;
            }
            else if (Input.GetButton("Up") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Up))
            {
                StartCoroutine(MakeStep(new Vector2(transform.position.x, transform.position.y + _mapCellSize), _speed));
                _currentPosInMap.y++;
            }
            else if (Input.GetButton("Down") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.Down))
            {
                StartCoroutine(MakeStep(new Vector2(transform.position.x, transform.position.y - _mapCellSize), _speed));
                _currentPosInMap.y--;
            }
        }
        catch (Exception ex) {

            GameManager.Instance.RestartGame();
        }



        
    }

}
