using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Maze2D.UI
{
    public class ColorToggle : MonoBehaviour
    {
        [SerializeField] 
        private Toggle _toggle;
        [SerializeField] 
        private Image _image;

        [field:SerializeField] 
        public UnityEvent<Color> OnColorSelected { get; private set; }

        public bool IsOn
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        }
        
        public Color Color => _image.color;
        
        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool value)
        {
            if (value)
            {
                OnColorSelected?.Invoke(_image.color);
            }
        }
    
        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}