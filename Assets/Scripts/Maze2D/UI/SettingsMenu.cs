using System;
using System.Collections.Generic;
using Maze2D.CodeBase.Controls;
using Maze2D.Controls;
using Maze2D.Extensions;
using Maze2D.Game;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Maze2D.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Slider _difficultySlider;
        [SerializeField] private ToggleGroup _playerColors;
        [SerializeField] private Button _backButton;
        
        [Inject] private IInputSystemService _inputSystemService;
        [Inject] private StorageService _storageService;
        
        private List<ColorToggle> _colorToggles;
        
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();

        private void Awake()
        {
            _difficultySlider.value = (float)_storageService.GetDifficulty();
            InitializePlayerColors();
        }

        private void InitializePlayerColors()
        {
            _colorToggles = new List<ColorToggle>(
                _playerColors.transform.GetComponentsInChildren<ColorToggle>(true));

            Color playerColor = _storageService.GetPlayerColor();

            int foundIndex = _colorToggles.FindIndex(toggle => toggle.Color.EqualsApproximately(playerColor));

            if (foundIndex == -1)
            {
                Debug.LogException(new InvalidOperationException("Default player color is not initialized"));
            }
            else
            {
                _colorToggles[foundIndex].IsOn = true;
            }
        }

        private void OnEnable()
        {
            _difficultySlider.OnValueChangedAsObservable().Subscribe(OnDifficultyChanged).AddTo(_disposables);

            foreach (ColorToggle toggle in _colorToggles)
            {
                toggle.OnColorSelected.AsObservable().Subscribe(OnPlayerColorSelected).AddTo(_disposables);
            }
            
            _backButton.OnClickAsObservable().Subscribe(u => OnBackClick()).AddTo(_disposables);
            
            Observable.EveryUpdate().Where(BackDemand)
                .Subscribe(u => OnBackClick())
                .AddTo(_disposables);
        }

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_difficultySlider.gameObject);
        }

        private void OnDifficultyChanged(float value)
        {
            _storageService.SetDifficulty((Difficulty)value);
        }
        
        private void OnPlayerColorSelected(Color color)
        {
            _storageService.SetPlayerColor(color);
        }

        private void OnBackClick()
        {
            
        }

        private bool BackDemand(long unit)
        {
            return _inputSystemService.GetButtonDown(InputActions.Back);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}