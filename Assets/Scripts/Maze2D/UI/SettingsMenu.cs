using System;
using System.Collections.Generic;
using Maze2D.CodeBase.Controls;
using Maze2D.CodeBase.Extensions;
using Maze2D.Controls;
using Maze2D.Domain;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
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
        
        private Settings _settings;
        private List<ColorToggle> _colorToggles;
        
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();

        [field: SerializeField] 
        public UnityEvent OnBack { get; private set; }
        
        private void Awake()
        {
            _settings = _storageService.Settings.Value;
            
            _difficultySlider.value = (float)_settings.GameDifficulty;
            InitializePlayerColors();
        }

        private void InitializePlayerColors()
        {
            _colorToggles = new List<ColorToggle>(
                _playerColors.transform.GetComponentsInChildren<ColorToggle>(true));
            
            Color32 playerColor = _settings.PlayerColor;
            int foundIndex = _colorToggles.FindIndex(toggle => playerColor.Equals((Color32)toggle.Color));

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
                .Subscribe(u => EventSystem.current.Submit(_backButton.gameObject))
                .AddTo(_disposables);

            SelectDefaultObject();
        }

        private void SelectDefaultObject()
        {
            EventSystem.current.SetSelectedGameObject(_difficultySlider.gameObject);
        }
        
        private void OnDifficultyChanged(float value)
        {
            _settings.GameDifficulty = (Difficulty)value;
            _settings.Save();
        }
        
        private void OnPlayerColorSelected(Color color)
        {
            _settings.PlayerColor = color;
            _settings.Save();
        }

        private void OnBackClick()
        {
            OnBack?.Invoke();
        }

        private bool BackDemand(long unit)
        {
            return _inputSystemService.GetButtonDown(InputActions.Back);
        }

        private void OnDisable()
        {
            _disposables.Clear();
            EventSystem.current.Release();
        }
    }
}