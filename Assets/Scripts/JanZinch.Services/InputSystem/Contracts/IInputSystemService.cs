namespace JanZinch.Services.InputSystem.Contracts
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