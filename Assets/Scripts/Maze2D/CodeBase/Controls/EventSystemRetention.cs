using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maze2D.CodeBase.Controls
{
    internal class EventSystemRetention : MonoBehaviour
    {
        [SerializeField] 
        private EventSystem _eventSystem;
        private GameObject _selectedObject;
        internal static EventSystemRetention Instance { get; private set; }
        
        public void Release()
        {
            _selectedObject = null;
            _eventSystem.SetSelectedGameObject(_selectedObject);
        }
        
        private void Reset()
        {
            _eventSystem = GetComponent<EventSystem>();
        }

        private void Awake()
        {
            Instance = this;
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