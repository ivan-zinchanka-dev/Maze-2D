using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class StartMenuPresenter : MonoBehaviour
{
    public const int MainMenuScene = 0;
    public const int GameSessionScene = 1;

    public static StartMenuPresenter Instance { get; private set; } = null;

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


    private PlayerColorToggle[] _colorToggles;
    
    
    private void Awake()
    {
        Instance = this;

        _colorToggles = _playerColors.transform.GetComponentsInChildren<PlayerColorToggle>(true);
        Color[] playerColors = _colorToggles.Select(colorToggle => colorToggle.Sprite.color).ToArray();
        
        StorageUtility.Initialize(playerColors);

        _difficultySlider.value = (float)StorageUtility.GetDifficulty();
        _difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
        
        Color currentColor = StorageUtility.GetPlayerColor();
        
        
        for (int i = 0; i < _colorToggles.Length; i++)
        {
            if (ApproximatelyEquals(_colorToggles[i].Sprite.color, currentColor, 0.001f))
            {
                Debug.Log("Match");
                _colorToggles[i].Toggle.isOn = true;
            }
            
            _colorToggles[i].OnColorSelected += OnPlayerColorChanged;
        }
        
    }
    

    private static void OnDifficultyChanged(float value)
    {
        StorageUtility.SetDifficulty((Difficulty)value);
    }
    
    private static void OnPlayerColorChanged(Color newColor)
    {
        StorageUtility.SetPlayerColor(newColor);
    }

    private static bool ApproximatelyEquals(Color first, Color second, float eps)
    {
        return ApproximatelyEquals(first.r, second.r, eps) &&
               ApproximatelyEquals(first.g, second.g, eps) &&
               ApproximatelyEquals(first.b, second.b, eps) &&
               ApproximatelyEquals(first.a, second.a, eps);
    }
    
    private static bool ApproximatelyEquals(float first, float second, float eps)
    {
        return Mathf.Abs(first - second) < eps;
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

    

    /*public void SetPlayerColor(ToggleGroup _toggleGroup) {

        List<Toggle> toggles = new List<Toggle>(_toggleGroup.transform.GetComponentsInChildren<Toggle>(true));
       
        foreach (Toggle currentToggle in toggles) {

            Debug.Log(currentToggle);

            if (currentToggle.transform.GetSiblingIndex() == GameManager.PlayerCustoms.ColorIndex) {

                currentToggle.isOn = true;
                break;
            }        
        }
    }*/

    public void ChangePlayerColor(PlayerColorToggle toggle) {

        /*if (toggle.Toggle.isOn)
        {
            GameManager.PlayerCustoms = new PlayerCustomisations()
            {
                RGBAColor = toggle.Sprite.color,
                ColorIndex = toggle.transform.GetSiblingIndex()
            };

            StorageUtility.SetPlayerColor(toggle.transform.GetSiblingIndex());
        }*/
        
    }

    /*
    private Color GetPlayerColorByIndex(int index)
    {
        List<PlayerColorToggle> toggles = new List<PlayerColorToggle>(_playerColors.transform.GetComponentsInChildren<PlayerColorToggle>(true));

        foreach (PlayerColorToggle currentToggle in toggles)
        {
            if (currentToggle.transform.GetSiblingIndex() == index)
            {
                return currentToggle.Sprite.color;
            }
        }

        return Color.white;
    }

    private void InitializeCache() {

        CacheUtility.DifficultyLevel = StorageUtility.GetDifficulty();
        CacheUtility.PlayerColor = GetPlayerColorByIndex(StorageUtility.GetPlayerColorIndex());
    }*/


    

}