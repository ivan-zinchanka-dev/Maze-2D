using Maze2D.CodeBase.Controls;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maze2D.CodeBase.Extensions
{
    public static class EventSystemExtensions
    {
        public static void Release(this EventSystem eventSystem)
        {
            if (eventSystem != null && eventSystem.TryGetComponent(out EventSystemRetention retention))
            {
                retention.Release();
            }
        }

        public static void Submit(this EventSystem eventSystem, GameObject gameObject)
        {
            ExecuteEvents.Execute(gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
        }
    }
}