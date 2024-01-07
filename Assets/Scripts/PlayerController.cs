using System;
using System.Collections;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private SpriteRenderer _spriteRender = null;

    private WallState[,] _map = null;
    private float _mapCellSize = default;
    private Vector2Int _currentPosInMap = default;
    private bool _canMove = true;

    
    public SpriteRenderer Sprite { get { return _spriteRender; } }    
   

    private Vector2Int WorldToMapCoords(Vector2 source) {

        if (source.y < 0)
        {
            if (source.x > 0) return _currentPosInMap = new Vector2Int(_map.GetLength(0) / 2 + 1, _map.GetLength(1) / 2 - 1);
            else return _currentPosInMap = new Vector2Int(_map.GetLength(0) / 2, _map.GetLength(1) / 2 - 1);
        }
        else
        {
            if (source.x > 0) return _currentPosInMap = new Vector2Int(_map.GetLength(0) / 2 + 1, _map.GetLength(1) / 2);
            else return _currentPosInMap = new Vector2Int(_map.GetLength(0) / 2, _map.GetLength(1) / 2);
        }        
    }


    public void Initialize(WallState[,] map, float mapCellSize, Vector2 startPosition)
    {
        _map = map;
        _canMove = true;
        _mapCellSize = mapCellSize;
       
        //transform.position = new Vector2(_mapCellSize / 2, _mapCellSize / 2);
        //_currentPosInMap = new Vector2Int(_map.GetLength(0) / 2 + 1, _map.GetLength(1) / 2);
        
        transform.position = startPosition;
        _currentPosInMap = WorldToMapCoords(startPosition);

    }

    public void ReInitialize(Vector2 startPosition) {

        _canMove = true;
        transform.position = startPosition;
        _currentPosInMap = WorldToMapCoords(startPosition);


    }

    private IEnumerator MakeStep(Vector2 target, float speed) {

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
            else if (Input.GetButton("Left") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.LEFT))
            {
                _canMove = false;
                StartCoroutine(MakeStep(new Vector2(transform.position.x - _mapCellSize, transform.position.y), _speed));
                _currentPosInMap.x--;

            }
            else if (Input.GetButton("Right") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.RIGHT))
            {
                _canMove = false;
                StartCoroutine(MakeStep(new Vector2(transform.position.x + _mapCellSize, transform.position.y), _speed));
                _currentPosInMap.x++;
            }
            else if (Input.GetButton("Up") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.UP))
            {
                _canMove = false;
                StartCoroutine(MakeStep(new Vector2(transform.position.x, transform.position.y + _mapCellSize), _speed));
                _currentPosInMap.y++;
            }
            else if (Input.GetButton("Down") && !_map[_currentPosInMap.x, _currentPosInMap.y].HasFlag(WallState.DOWN))
            {
                _canMove = false;
                StartCoroutine(MakeStep(new Vector2(transform.position.x, transform.position.y - _mapCellSize), _speed));
                _currentPosInMap.y--;
            }


        }
        catch (Exception ex) {

            GameManager.Instance.RestartGame();
        }



        
    }

}
