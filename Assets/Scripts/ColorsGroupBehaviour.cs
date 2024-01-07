using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorsGroupBehaviour : MonoBehaviour
{
    [SerializeField] private ToggleGroup _toggleGroup = null;

    public void UpdateGroupView()
    {
        List<Toggle> toggles = new List<Toggle>(_toggleGroup.transform.GetComponentsInChildren<Toggle>(true));

        foreach (Toggle currentToggle in toggles) {

            currentToggle.isOn = false;
        }

        foreach (Toggle currentToggle in toggles)
        {
            if (currentToggle.transform.GetSiblingIndex() == GameManager.PlayerCustoms.ColorIndex)
            {
                currentToggle.isOn = true;
                break;
            }
        }
    }

    private void OnEnable()
    {
        UpdateGroupView();    
    }


}