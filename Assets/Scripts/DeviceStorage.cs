using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class DeviceStorage
{
    private const string DifficultyKey = "difficulty";
    private const string PlayerColorKey = "color";
    private const int Default = -1;


    public static void WritePlayerColorIndex(int playerColorIndex) {

        PlayerPrefs.SetInt(PlayerColorKey, playerColorIndex);
        PlayerPrefs.Save();    
    }

    public static PlayerCustomisations ReadPlayerCustoms(ToggleGroup colorsGroup)
    {
        int index = PlayerPrefs.GetInt(PlayerColorKey, Default);

        if (index == Default) { 
            
            throw new DeviceStorageException(); 
        }
        else
        {
            List<PlayerColorToggle> toggles = new List<PlayerColorToggle>(colorsGroup.transform.GetComponentsInChildren<PlayerColorToggle>(true));

            foreach (PlayerColorToggle currentToggle in toggles)
            {
                if (currentToggle.transform.GetSiblingIndex() == index)
                {
                    return new PlayerCustomisations { ColorIndex = index, RGBAColor = currentToggle.Sprite.color };
                }
            }

            throw new DeviceStorageException();
        }        
    }

    public static void WriteDifficulty(Difficulty difficultyLevel) {

        PlayerPrefs.SetInt(DifficultyKey, (int)difficultyLevel);
        PlayerPrefs.Save();
    }

    public static Difficulty ReadDifficulty() {

        int result = PlayerPrefs.GetInt(DifficultyKey, Default);
        if (result == Default) throw new DeviceStorageException();
        else return (Difficulty)result;
    }

}

public class DeviceStorageException : Exception { }