using Maze2D.CodeBase.Controls;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Maze2D.CodeBase.View
{
    public abstract class View : MonoBehaviour
    {
        [Inject] private IInputSystemService _inputSystemService;
        
        private bool _isActive;
        
        public void Activate()
        {
            _isActive = true;


        }

        protected abstract Selectable DefaultSelectable { get; }



    }
}