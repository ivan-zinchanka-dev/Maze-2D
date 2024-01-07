using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSelectRetention : MonoBehaviour
{
    [SerializeField] private EventSystem _eventSystem = null;

    private GameObject _selectedObject = null;

    private void Start()
    {
        if (_eventSystem == null) throw new NullReferenceException("Event system not set!");
    }

    private void Update()
    {
        if (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject != _selectedObject)
        {
            _selectedObject = _eventSystem.currentSelectedGameObject;
        }
        else {
           
            _eventSystem.SetSelectedGameObject(_selectedObject);
        }
    }
}