using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerColorToggle : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle = null;
        [SerializeField] private Image _image = null;

        public event Action<Color> OnColorSelected;
    
        public Toggle Toggle { get { return _toggle; } }
        public Image Sprite { get { return _image; } }


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