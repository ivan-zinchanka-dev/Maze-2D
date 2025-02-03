using System;
using Maze2D.CodeBase.Controls;
using Maze2D.Controls;
using UnityEngine;
using VContainer;

namespace Maze2D.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] 
        private PlayerView _playerView;

        [Inject] 
        private IInputSystemService _inputSystemService;
        
        public event Action Finished;

        private void Update()
        {
            //if (!_canMove || GameManager.Instance.Pause) return;

            if (_playerView.IsFinished())
            {
                Finished?.Invoke();
                //GameManager.Instance.RegenerateMaze();
            }
            else if (_inputSystemService.GetButton(InputActions.Right))
            {
                _playerView.StepRight();
            }
            else if (_inputSystemService.GetButton(InputActions.Left))
            {
                _playerView.StepLeft();
            }
            else if (_inputSystemService.GetButton(InputActions.Up))
            {
                _playerView.StepUp();
            }
            else if (_inputSystemService.GetButton(InputActions.Down))
            {
                _playerView.StepDown();
            }
        }
    }
}