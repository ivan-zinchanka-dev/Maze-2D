using Maze2D.CodeBase.Controls;
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
    }
}