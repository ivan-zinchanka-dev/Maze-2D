namespace Maze2D.CodeBase.Controls
{
    public interface IInputSystemService
    {
        public bool IsActive { get; set; }
        public bool GetButton(string actionName);
        public bool GetButtonUp(string actionName);
        public bool GetButtonDown(string actionName);
        public float GetAxis(string actionName);
    }
}