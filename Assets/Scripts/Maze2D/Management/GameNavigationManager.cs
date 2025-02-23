using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JanZinch.Services.Logging.Contracts;
using JanZinch.Services.Logging.Contracts.Generic;
using JanZinch.Services.Logging.Extensions;
using Maze2D.UI;
using Maze2D.View;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Maze2D.Management
{
    internal class GameNavigationManager : MonoBehaviour
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
        private ILogger<GameNavigationManager> _logger;
        
        [Serializable]
        private struct ViewHolders {

            public RectTransform Left;
            public RectTransform Center;
            public RectTransform Right;
        }

        [Inject]
        private void InjectLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GameNavigationManager>();
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
            pauseMenu.MainMenuCommand.Subscribe(u => ExitToMainMenuAsync()).AddTo(disposables);
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
            const string processName = "Game starting";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            _mainMenuDisposables.Clear();
            
            Sequence tween = _viewNavigationService.HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left)
                .AppendCallback(() => _gameStateMachine.PlayAsync(NextLevelAsync).Forget());

            _logger.LogDebug(GetProcessCompletedMessage(processName));
            return tween;
        }
        
        private Sequence ResumeGame()
        {
            const string processName = "Game resuming";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            _pauseMenuDisposables.Clear();
            
            Sequence tween = _viewNavigationService.HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right)
                .AppendCallback(() => _gameStateMachine.Continue());
            
            _logger.LogDebug(GetProcessCompletedMessage(processName));
            return tween;
        }
        
        private async void RestartGameAsync()
        {
            const string processName = "Game restarting";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            
            _pauseMenuDisposables.Clear();
            
            UniTask hidePlayerTask = _gameStateMachine.HidePlayerAsync();
            UniTask hidePauseMenuTask = _viewNavigationService
                .HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();

            await UniTask.WhenAll(hidePlayerTask, hidePauseMenuTask);
            
            _gameStateMachine.RestartLevel();
            await _gameStateMachine.ShowPlayerAsync();
            
            _logger.LogDebug(GetProcessCompletedMessage(processName));
        }

        private async void RegenerateLevelAsync()
        {
            const string processName = "Level regeneration";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            
            _pauseMenuDisposables.Clear();
            
            UniTask hidePauseMenuTask = _viewNavigationService
                .HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();
            await _gameStateMachine.HidePlayerAsync();
            
            await _gameStateMachine.ExitAsync();
            await hidePauseMenuTask;
            
            OnLevelRegeneration?.Invoke();
            await _gameStateMachine.PlayAsync(NextLevelAsync);
            
            _logger.LogDebug(GetProcessCompletedMessage(processName));
        }
        
        private async void NextLevelAsync()
        {
            const string processName = "Moving to next level";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            OnLevelFinished?.Invoke();
            
            await _gameStateMachine.HidePlayerAsync();
            await _gameStateMachine.ExitAsync();
            await _gameStateMachine.PlayAsync(NextLevelAsync);
            
            OnLevelRegeneration?.Invoke();
            _logger.LogDebug(GetProcessCompletedMessage(processName));
        }

        private async void ExitToMainMenuAsync()
        {
            const string processName = "Exiting to main menu";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            _pauseMenuDisposables.Clear();
            
            UniTask exitTask = _gameStateMachine.ExitAsync();
            UniTask navigateTask = NavigateToMainMenu(_pauseMenu).ToUniTask();
            
            await UniTask.WhenAll(exitTask, navigateTask);
            _logger.LogDebug(GetProcessCompletedMessage(processName));
        }
        
        private Sequence NavigateToPauseMenu()
        {
            const string processName = "Navigating to pause menu";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            
            if (_pauseMenu == null)
            {
                _pauseMenu = _viewFactory.CreateView<PauseMenu>();
                SubscribeToPauseMenuCommands(_pauseMenu, _pauseMenuDisposables);
            }
            
            OnPause?.Invoke();
            Sequence tween = _viewNavigationService
                .ShowView(_pauseMenu, _viewHolders.Right, _viewHolders.Center);
            
            _logger.LogDebug(GetProcessCompletedMessage(processName));
            return tween;
        }

        private Sequence NavigateToSettings()
        {
            const string processName = "Navigating to settings menu";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            
            SettingsMenu settingsMenu = _viewFactory.CreateView<SettingsMenu>();
            settingsMenu.OnBack.AddListener(() =>
            {
                NavigateToMainMenu(settingsMenu);
            });
            
            _mainMenuDisposables.Clear();
            
            Sequence tween = DOTween.Sequence()
                .Append(_viewNavigationService.HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left))
                .Join(_viewNavigationService.ShowView(settingsMenu, _viewHolders.Right, _viewHolders.Center));
            
            _logger.LogDebug(GetProcessCompletedMessage(processName));
            return tween;
        }

        private Sequence NavigateToMainMenu(MonoBehaviour currentView)
        {
            const string processName = "Navigating to main menu";
            _logger.LogDebug(GetProcessBegunMessage(processName));
            
            if (_mainMenu == null)
            {
                _mainMenu = _viewFactory.CreateView<MainMenu>();
                SubscribeToMainMenuCommands(_mainMenu, _mainMenuDisposables);
            }

            Sequence tween =  DOTween.Sequence()
                .Append(_viewNavigationService.HideView(currentView, _viewHolders.Center, _viewHolders.Right))
                .Join(_viewNavigationService.ShowView(_mainMenu, _viewHolders.Left, _viewHolders.Center));

            _logger.LogDebug(GetProcessCompletedMessage(processName));
            return tween;
        }
        
        private static string GetProcessBegunMessage(string processName)
        {
            return processName + " has begun";
        }

        private static string GetProcessCompletedMessage(string processName)
        {
            return processName + " completed";
        }
        
        private void OnDisable()
        {
            _stateMachineDisposables.Clear();
            _mainMenuDisposables.Clear();
            _pauseMenuDisposables.Clear();
        }
    }
}