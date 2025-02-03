using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maze2D.CodeBase.Controls
{
    public class EventSystemRetention : MonoBehaviour
    {
        [SerializeField] private EventSystem _eventSystem = null;

        private GameObject _selectedObject = null;

        private void Reset()
        {
            _eventSystem = GetComponent<EventSystem>();
        }

        private void Start()
        {
            if (_eventSystem == null) throw new NullReferenceException("Event system not set!");
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