using UnityEngine;
using UnityEngine.EventSystems;

namespace JanZinch.Services.InputSystem.Retention
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemRetention : MonoBehaviour
    {
        [SerializeField] 
        private EventSystem _eventSystem;
        private GameObject _selectedObject;
        
        internal void Release()
        {
            _selectedObject = null;
            _eventSystem.SetSelectedGameObject(_selectedObject);
        }
        
        private void Reset()
        {
            _eventSystem = GetComponent<EventSystem>();
        }

        private void Update()
        {
            if (_eventSystem.currentSelectedGameObject != null && 
                _eventSystem.currentSelectedGameObject != _selectedObject)
            {
                _selectedObject = _eventSystem.currentSelectedGameObject;
            }
            else {
           
                _eventSystem.SetSelectedGameObject(_selectedObject);
            }
        }
    }
}