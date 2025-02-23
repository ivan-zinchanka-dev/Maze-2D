using System;
using System.Collections.Generic;
using JanZinch.Services.InputSystem.Contracts;
using JanZinch.Services.InputSystem.Retention;
using Maze2D.Controls;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Maze2D.UI
{
    internal class PauseMenu : MonoBehaviour
    {
        [SerializeField] 
        private Button _resumeButton;
        [SerializeField] 
        private Button _restartLevelButton;
        [SerializeField] 
        private Button _newLevelButton;
        [SerializeField] 
        private Button _mainMenuButton;
        
        [Inject] 
        private IInputSystemService _inputSystemService;
        
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        public IReactiveCommand<Unit> ResumeCommand { get; private set; }
        public IReactiveCommand<Unit> RestartLevelCommand { get; private set; }
        public IReactiveCommand<Unit> NewLevelCommand { get; private set; }
        public IReactiveCommand<Unit> MainMenuCommand { get; private set; }
        
        private void Awake()
        {
            ResumeCommand = new ReactiveCommand<Unit>();
            RestartLevelCommand = new ReactiveCommand<Unit>();
            NewLevelCommand = new ReactiveCommand<Unit>();
            MainMenuCommand = new ReactiveCommand<Unit>();
        }

        private void OnEnable()
        {
            Bind(_resumeButton, ResumeCommand);
            Bind(_restartLevelButton, RestartLevelCommand);
            Bind(_newLevelButton, NewLevelCommand);
            Bind(_mainMenuButton, MainMenuCommand);
            
            Observable.EveryUpdate()
                .Where(ResumeGameDemand)
                .Subscribe(unit => EventSystem.current.Submit(_resumeButton.gameObject))
                .AddTo(_disposables);

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
            EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
        }
        
        private bool ResumeGameDemand(long unit)
        {
            return _inputSystemService.GetButtonDown(InputActions.Pause);
        }

        private void OnDisable()
        {
            _disposables.Clear();
            EventSystem.current.Release();
        }
    }
}