using UnityEngine;

namespace Maze2D.CodeBase.Controls
{
    public class InputSystemService : IInputSystemService
    {
        public bool IsActive { get; set; }
        public bool GetButton(string actionName)
        {
            return IsActive && Input.GetButton(actionName);
        }

        public bool GetButtonUp(string actionName)
        {
            return IsActive && Input.GetButtonUp(actionName);
        }

        public bool GetButtonDown(string actionName)
        {
            return IsActive && Input.GetButtonDown(actionName);
        }
        
        public float GetAxis(string actionName)
        {
            return IsActive ? Input.GetAxis(actionName) : 0.0f;
        }
    }
}