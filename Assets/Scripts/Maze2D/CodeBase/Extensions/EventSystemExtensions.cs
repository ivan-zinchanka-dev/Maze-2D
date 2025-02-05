using Maze2D.CodeBase.Controls;
using UnityEngine.EventSystems;

namespace Maze2D.CodeBase.Extensions
{
    public static class EventSystemExtensions
    {
        public static void Release(this EventSystem eventSystem)
        {
            if (EventSystemRetention.Instance != null)
            {
                EventSystemRetention.Instance.Release();
            }
        }
    }
}