using UnityEngine.UI;
using UnityEngine;

public class PlayerColorToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle = null;
    [SerializeField] private Image _image = null;

    public Toggle Info { get { return _toggle; } }
    public Image Sprite { get { return _image; } }

}