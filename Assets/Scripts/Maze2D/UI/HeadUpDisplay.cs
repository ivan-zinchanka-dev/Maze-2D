using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Maze2D.CodeBase.View;
using Maze2D.Management;
using UniRx;
using UnityEngine;
using VContainer;

namespace Maze2D.UI
{
    public class HeadUpDisplay : MonoBehaviour
    {
        [SerializeField] 
        private float _viewNavigationDuration = 0.75f;
        [SerializeField] 
        private ViewHolders _viewHolders;
        
        [Inject] 
        private ViewFactory _viewFactory;
        [Inject] 
        private GameStateMachine _gameStateMachine;
        
        
        private MainMenu _mainMenu;
        private PauseMenu _pauseMenu;
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        
        [Serializable]
        private struct ViewHolders {

            public RectTransform Left;
            public RectTransform Center;
            public RectTransform Right;
        }
        
        private void Awake()
        {
            _gameStateMachine.CurrentState.Subscribe(OnGameStateChanged).AddTo(_disposables);
            
            _mainMenu = _viewFactory.CreateView<MainMenu>();
        }

        private void OnEnable()
        {
            _mainMenu.CommandInvoked.AddListener(ProcessMainMenuCommand);
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

        
        private void ProcessPauseMenuCommand(PauseMenu.CommandKind command)
        {
            switch (command)
            {
                case PauseMenu.CommandKind.RestartLevel:
                    RestartGameAsync();
                    break;
                case PauseMenu.CommandKind.RegenerateLevel:
                    RegenerateLevelAsync();
                    break;
                case PauseMenu.CommandKind.ToMainMenu:
                    ExitGameAsync();
                    break;
                case PauseMenu.CommandKind.Continue:
                    ResumeGame();
                    break;
            }
        }
        
        private void ProcessMainMenuCommand(MainMenu.CommandKind command)
        {
            switch (command)
            {
                case MainMenu.CommandKind.Play:
                    StartGame();
                    break;
                case MainMenu.CommandKind.Settings:
                    NavigateToSettings(_mainMenu);
                    break;
                case MainMenu.CommandKind.Exit:
                    Application.Quit();
                    break;
            }
        }
        
        private Sequence StartGame()
        {
            return HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left)
                .AppendCallback(() => _gameStateMachine.PlayAsync().Forget());
        }
        
        private async void RestartGameAsync()
        {
            UniTask hidePlayerTask = _gameStateMachine.HidePlayerAsync();
            UniTask hidePauseMenuTask = HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();

            await UniTask.WhenAll(hidePlayerTask, hidePauseMenuTask);
            
            _gameStateMachine.RestartLevel();
            await _gameStateMachine.ShowPlayerAsync();
        }

        private async void RegenerateLevelAsync()
        {
            UniTask hidePauseMenuTask = HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();
            await _gameStateMachine.HidePlayerAsync();
            
            await _gameStateMachine.ExitAsync();
            await hidePauseMenuTask;
            
            await _gameStateMachine.PlayAsync();
        }

        private async void ExitGameAsync()
        {
            UniTask exitTask = _gameStateMachine.ExitAsync();
            UniTask navigateTask = NavigateToMainMenu(_pauseMenu).ToUniTask();
            
            await UniTask.WhenAll(exitTask, navigateTask);
        }

        private Sequence ResumeGame()
        {
            return HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right)
                .AppendCallback(() => _gameStateMachine.Continue());
        }
        
        private Sequence NavigateToPauseMenu()
        {
            if (_pauseMenu == null)
            {
                _pauseMenu = _viewFactory.CreateView<PauseMenu>();
                _pauseMenu.CommandInvoked.AddListener(ProcessPauseMenuCommand);
            }
            
            return ShowView(_pauseMenu, _viewHolders.Right, _viewHolders.Center);
        }

        private Sequence NavigateToSettings(MonoBehaviour currentView)
        {
            SettingsMenu settingsMenu = _viewFactory.CreateView<SettingsMenu>();
            settingsMenu.OnBack.AddListener(() =>
            {
                NavigateToMainMenu(settingsMenu);
            });
            
            return DOTween.Sequence()
                .Append(HideView(currentView, _viewHolders.Center, _viewHolders.Left))
                .Join(ShowView(settingsMenu, _viewHolders.Right, _viewHolders.Center));
        }

        private Sequence NavigateToMainMenu(MonoBehaviour currentView)
        {
            if (_mainMenu == null)
            {
                _mainMenu = _viewFactory.CreateView<MainMenu>();
                _mainMenu.CommandInvoked.AddListener(ProcessMainMenuCommand);
            }

            return DOTween.Sequence()
                .Append(HideView(currentView, _viewHolders.Center, _viewHolders.Right))
                .Join(ShowView(_mainMenu, _viewHolders.Left, _viewHolders.Center));
        }

        
        private Sequence ShowView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            return MoveView(view, from, to)
                .AppendCallback(() =>
                {
                    view.enabled = true;
                });
        }
        
        private Sequence HideView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            return MoveView(view, from, to)
                .AppendCallback(() =>
                {
                    Destroy(view.gameObject);
                });
        }

        private Sequence MoveView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            view.enabled = false;
            view.transform.localPosition = from.localPosition;
            
            return DOTween.Sequence()
                .Append(view.transform
                    .DOLocalMoveX(to.localPosition.x, _viewNavigationDuration)
                    .SetEase(Ease.OutSine))
                .SetUpdate(true)
                .SetLink(view.gameObject);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}