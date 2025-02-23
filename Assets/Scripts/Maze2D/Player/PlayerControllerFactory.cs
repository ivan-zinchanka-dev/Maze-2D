using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Maze2D.Player
{
    internal class PlayerControllerFactory : MonoBehaviour
    {
        [SerializeField] 
        private PlayerController _playerPrefab;
        [Inject] 
        private IObjectResolver _container;
        
        public PlayerController CreatePlayer()
        {
            return _container.Instantiate(_playerPrefab);
        }
    }
}