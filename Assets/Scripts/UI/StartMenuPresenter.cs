using System;
using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class StartMenuPresenter : MonoBehaviour
    {
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
        
        private ColorToggle[] _colorToggles;
        
        private void Awake()
        {
            Instance = this;
            _colorToggles = _playerColors.transform.GetComponentsInChildren<ColorToggle>(true);
        }

        private void Start()
        {
            _difficultySlider.value = (float)StorageUtility.GetDifficulty();
            _difficultySlider.onValueChanged.AddListener(OnDifficultyChanged);
            
            Color currentColor = StorageUtility.GetPlayerColor();

            if (currentColor == default)
            {
                _colorToggles[0].IsOn = true;
                StorageUtility.SetPlayerColor(_colorToggles[0].Color);

                foreach (var colorToggle in _colorToggles)
                {
                    colorToggle.OnColorSelected += OnPlayerColorChanged;
                }
            }
            else
            {
                foreach (var colorToggle in _colorToggles)
                {
                    colorToggle.IsOn = 
                        ApproximatelyEquals(colorToggle.Color, currentColor, 0.001f);
                    
                    colorToggle.OnColorSelected += OnPlayerColorChanged;
                }
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

            SceneManager.LoadScene(ScenesUtility.GameSceneIndex);
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


        public void ShowSettings() {

            StartCoroutine(Move(_mainMenu, _menuPositions.Left, () => _mainMenu.gameObject.SetActive(false)));
            _settingsMenu.gameObject.SetActive(true);
            StartCoroutine(Move(_settingsMenu, _menuPositions.Center));
            EventSystem.current.SetSelectedGameObject(_difficultySlider.gameObject);
        }

        public void ShowMainMenu()
        {
            StartCoroutine(Move(_settingsMenu, _menuPositions.Right, () => _settingsMenu.gameObject.SetActive(false)));
            _mainMenu.gameObject.SetActive(true);
            StartCoroutine(Move(_mainMenu, _menuPositions.Center));
            EventSystem.current.SetSelectedGameObject(_settingsButton.gameObject);
        }
        
    }
}