using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ColorToggleOld : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private Image _image = null;
        
        public bool IsOn
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        }
        
        public Color Color => _image.color;
        public event Action<Color> OnColorSelected;
        
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