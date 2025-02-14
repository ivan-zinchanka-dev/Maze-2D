using Maze2D.CodeBase.Controls;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maze2D.CodeBase.Extensions
{
    public static class EventSystemExtensions
    {
        public static void Release(this EventSystem eventSystem)
        {
            if (eventSystem.TryGetComponent<EventSystemRetention>(out EventSystemRetention retention))
            {
                retention.Release();
            }
        }
        
        public static bool ReleaseGameObject(this EventSystem eventSystem, GameObject gameObject)
        {
            if (eventSystem.currentSelectedGameObject == gameObject)
            {
                Release(eventSystem);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}