using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class GUIManager : MonoBehaviour
{
    public const int MainMenuScene = 0;
    public const int GameSessionScene = 1;

    public static GUIManager Instance { get; private set; } = null;

    [SerializeField] private RectTransform _mainMenu = null;
    [SerializeField] private RectTransform _settingsMenu = null;
    [SerializeField] private Slider _difficultySlider = null;
    [SerializeField] private Button _settingsButton = null;
    [SerializeField] private ToggleGroup _playerColors = null;
    [SerializeField] private float _buttonSpeed = 2.0f;

    [SerializeField] private MovingConstraints _menuPositions = default;

    [Serializable]
    private struct MovingConstraints {

        public RectTransform Left;
        public RectTransform Center;
        public RectTransform Right;
    }


    public void StartGame() {

        SceneManager.LoadScene(GameSessionScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator Move(RectTransform current, RectTransform target, Action callback = null) {

        while (current.anchoredPosition != target.anchoredPosition) {

            current.anchoredPosition = Vector2.MoveTowards(current.anchoredPosition, target.anchoredPosition, _buttonSpeed * Time.deltaTime);
            yield return null;
        }

        callback?.Invoke();

        yield return null;
    }


    public void ViewSettings() {

        StartCoroutine(Move(_mainMenu, _menuPositions.Left, () => _mainMenu.gameObject.SetActive(false)));
        _settingsMenu.gameObject.SetActive(true);
        StartCoroutine(Move(_settingsMenu, _menuPositions.Center));
        EventSystem.current.SetSelectedGameObject(_difficultySlider.gameObject);
    }

    public void ViewMainMenu()
    {
        StartCoroutine(Move(_settingsMenu, _menuPositions.Right, () => _settingsMenu.gameObject.SetActive(false)));
        _mainMenu.gameObject.SetActive(true);
        StartCoroutine(Move(_mainMenu, _menuPositions.Center));
        EventSystem.current.SetSelectedGameObject(_settingsButton.gameObject);
    }

    public void SetDifficultyLevel(Slider difficultySlider)
    {        
        difficultySlider.value = (float)GameManager.DifficultyLevel;
    }

    public void ChangeDifficultyLevel(Slider difficultySlider)
    {       
        GameManager.DifficultyLevel = (Difficulty)difficultySlider.value;
        DeviceStorage.WriteDifficulty(GameManager.DifficultyLevel);
    }

    public void SetPlayerColor(ToggleGroup _toggleGroup) {

        List<Toggle> toggles = new List<Toggle>(_toggleGroup.transform.GetComponentsInChildren<Toggle>(true));
       
        foreach (Toggle currentToggle in toggles) {

            Debug.Log(currentToggle);

            if (currentToggle.transform.GetSiblingIndex() == GameManager.PlayerCustoms.ColorIndex) {

                currentToggle.isOn = true;
                break;
            }        
        }
    }

    public void ChangePlayerColor(PlayerColorToggle toggle) {

        if (toggle.Info.isOn)
        {
            GameManager.PlayerCustoms = new PlayerCustomisations()
            {
                RGBAColor = toggle.Sprite.color,
                ColorIndex = toggle.transform.GetSiblingIndex()
            };

            DeviceStorage.WritePlayerColorIndex(toggle.transform.GetSiblingIndex());
        }
        
    }

    private void InitPlayerData() {

        try
        {
            GameManager.DifficultyLevel = DeviceStorage.ReadDifficulty();
            GameManager.PlayerCustoms = DeviceStorage.ReadPlayerCustoms(_playerColors);
        }
        catch (DeviceStorageException)
        {
            GameManager.DifficultyLevel = Difficulty.EASY;
            GameManager.PlayerCustoms = PlayerCustomisations.Default;
        }
    }


    private void Awake()
    {
        Instance = this;
        InitPlayerData();

        SetDifficultyLevel(_difficultySlider);
        SetPlayerColor(_playerColors);
    }

}