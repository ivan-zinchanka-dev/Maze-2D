using JanZinch.Services.InputSystem.Contracts;
using Maze2D.Controls;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Maze2D.Player
{
    internal class PlayerController : MonoBehaviour
    {
        [SerializeField] 
        private PlayerView _playerView;
        [field:SerializeField] 
        public UnityEvent Finished { get; private set; }

        [Inject] 
        private IInputSystemService _inputSystemService;

        public PlayerView View => _playerView;

        private void Reset()
        {
            _playerView = GetComponent<PlayerView>();
        }
        
        private void Update()
        {
            if (_playerView.IsFinished())
            {
                Finished?.Invoke();
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