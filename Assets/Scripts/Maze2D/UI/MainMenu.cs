using System;
using System.Collections.Generic;
using JanZinch.Services.InputSystem.Retention;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maze2D.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] 
        private Button _playButton;
        [SerializeField] 
        private Button _settingsButton;
        [SerializeField] 
        private Button _exitButton;
        
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        public IReactiveCommand<Unit> PlayCommand { get; private set; }
        public IReactiveCommand<Unit> SettingsCommand { get; private set; }
        public IReactiveCommand<Unit> ExitCommand { get; private set; }
        
        private void Awake()
        {
            PlayCommand = new ReactiveCommand<Unit>();
            SettingsCommand = new ReactiveCommand<Unit>();
            ExitCommand = new ReactiveCommand<Unit>();
        }

        private void OnEnable()
        {
            Bind(_playButton, PlayCommand);
            Bind(_settingsButton, SettingsCommand);
            Bind(_exitButton, ExitCommand);
            
            SelectDefaultObject();
        }

        private void Bind(Button button, IReactiveCommand<Unit> command)
        {
            button.OnClickAsObservable()
                .Subscribe(unit => command.Execute(unit))
                .AddTo(_disposables);
        }
        
        private void SelectDefaultObject()
        {
            EventSystem.current.SetSelectedGameObject(_playButton.gameObject);
        }

        private void OnDisable()
        {
            _disposables.Clear();
            EventSystem.current.Release();
        }
    }
}