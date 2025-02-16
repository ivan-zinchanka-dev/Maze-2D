using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Maze2D.CodeBase.View;
using Maze2D.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Maze2D.Management
{
    public class GameNavigationManager : MonoBehaviour
    {
        [SerializeField] 
        private float _viewNavigationDuration = 0.4f;
        [SerializeField] 
        private ViewHolders _viewHolders;

        [field: Space]
        [field: SerializeField]
        public UnityEvent OnPause { get; private set; }
        [field: SerializeField]
        public UnityEvent OnLevelRegeneration { get; private set; }
        [field: SerializeField]
        public UnityEvent OnLevelFinished { get; private set; }
        
        [Inject] 
        private ViewFactory _viewFactory;
        [Inject] 
        private GameStateMachine _gameStateMachine;
        
        private readonly ICollection<IDisposable> _stateMachineDisposables = new CompositeDisposable();
        private readonly ICollection<IDisposable> _mainMenuDisposables = new CompositeDisposable();
        private readonly ICollection<IDisposable> _pauseMenuDisposables = new CompositeDisposable();

        private ViewNavigationService _viewNavigationService;
        private MainMenu _mainMenu;
        private PauseMenu _pauseMenu;
        
        [Serializable]
        private struct ViewHolders {

            public RectTransform Left;
            public RectTransform Center;
            public RectTransform Right;
        }
        
        private void Awake()
        {
            _viewNavigationService = new ViewNavigationService(_viewNavigationDuration);
            _mainMenu = _viewFactory.CreateView<MainMenu>();
        }

        private void OnEnable()
        {
            _gameStateMachine.CurrentState.Subscribe(OnGameStateChanged).AddTo(_stateMachineDisposables);
            SubscribeToMainMenuCommands(_mainMenu, _mainMenuDisposables);
        }
        
        private void SubscribeToMainMenuCommands(MainMenu mainMenu, ICollection<IDisposable> disposables)
        {
            mainMenu.PlayCommand.Subscribe(u => StartGame()).AddTo(disposables);
            mainMenu.SettingsCommand.Subscribe(u => NavigateToSettings()).AddTo(disposables);
            mainMenu.ExitCommand.Subscribe(u => Application.Quit()).AddTo(disposables);
        }
        
        private void SubscribeToPauseMenuCommands(PauseMenu pauseMenu, ICollection<IDisposable> disposables)
        {
            pauseMenu.ResumeCommand.Subscribe(u => ResumeGame()).AddTo(disposables);
            pauseMenu.RestartLevelCommand.Subscribe(u => RestartGameAsync()).AddTo(disposables);
            pauseMenu.NewLevelCommand.Subscribe(u => RegenerateLevelAsync()).AddTo(disposables);
            pauseMenu.MainMenuCommand.Subscribe(u => ExitGameAsync()).AddTo(disposables);
        }
        
        private void OnGameStateChanged(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Paused:
                    NavigateToPauseMenu();
                    break;
            }
        }
        
        private Sequence StartGame()
        {
            _mainMenuDisposables.Clear();
            
            return _viewNavigationService.HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left)
                .AppendCallback(() => _gameStateMachine.PlayAsync(NextLevelAsync).Forget());
        }
        
        private async void RestartGameAsync()
        {
            _pauseMenuDisposables.Clear();
            
            UniTask hidePlayerTask = _gameStateMachine.HidePlayerAsync();
            UniTask hidePauseMenuTask = _viewNavigationService
                .HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();

            await UniTask.WhenAll(hidePlayerTask, hidePauseMenuTask);
            
            _gameStateMachine.RestartLevel();
            await _gameStateMachine.ShowPlayerAsync();
        }

        private async void RegenerateLevelAsync()
        {
            _pauseMenuDisposables.Clear();
            
            UniTask hidePauseMenuTask = _viewNavigationService
                .HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();
            await _gameStateMachine.HidePlayerAsync();
            
            await _gameStateMachine.ExitAsync();
            await hidePauseMenuTask;
            
            OnLevelRegeneration?.Invoke();
            
            await _gameStateMachine.PlayAsync(NextLevelAsync);
        }
        
        private async void NextLevelAsync()
        {
            OnLevelFinished?.Invoke();
            
            await _gameStateMachine.HidePlayerAsync();
            await _gameStateMachine.ExitAsync();
            await _gameStateMachine.PlayAsync(NextLevelAsync);
        }

        private async void ExitGameAsync()
        {
            _pauseMenuDisposables.Clear();
            
            UniTask exitTask = _gameStateMachine.ExitAsync();
            UniTask navigateTask = NavigateToMainMenu(_pauseMenu).ToUniTask();
            
            await UniTask.WhenAll(exitTask, navigateTask);
        }

        private Sequence ResumeGame()
        {
            _pauseMenuDisposables.Clear();
            
            return _viewNavigationService.HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right)
                .AppendCallback(() => _gameStateMachine.Continue());
        }
        
        private Sequence NavigateToPauseMenu()
        {
            if (_pauseMenu == null)
            {
                _pauseMenu = _viewFactory.CreateView<PauseMenu>();
                SubscribeToPauseMenuCommands(_pauseMenu, _pauseMenuDisposables);
            }
            
            OnPause?.Invoke();
            
            return _viewNavigationService.ShowView(_pauseMenu, _viewHolders.Right, _viewHolders.Center);
        }

        private Sequence NavigateToSettings()
        {
            SettingsMenu settingsMenu = _viewFactory.CreateView<SettingsMenu>();
            settingsMenu.OnBack.AddListener(() =>
            {
                NavigateToMainMenu(settingsMenu);
            });
            
            _mainMenuDisposables.Clear();
            
            return DOTween.Sequence()
                .Append(_viewNavigationService.HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left))
                .Join(_viewNavigationService.ShowView(settingsMenu, _viewHolders.Right, _viewHolders.Center));
        }

        private Sequence NavigateToMainMenu(MonoBehaviour currentView)
        {
            if (_mainMenu == null)
            {
                _mainMenu = _viewFactory.CreateView<MainMenu>();
                SubscribeToMainMenuCommands(_mainMenu, _mainMenuDisposables);
            }

            return DOTween.Sequence()
                .Append(_viewNavigationService.HideView(currentView, _viewHolders.Center, _viewHolders.Right))
                .Join(_viewNavigationService.ShowView(_mainMenu, _viewHolders.Left, _viewHolders.Center));
        }
        
        private void OnDisable()
        {
            _stateMachineDisposables.Clear();
            _mainMenuDisposables.Clear();
            _pauseMenuDisposables.Clear();
        }
    }
}