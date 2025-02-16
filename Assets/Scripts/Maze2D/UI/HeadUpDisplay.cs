using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Maze2D.Audio;
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
        private float _viewNavigationDuration = 0.25f;
        [SerializeField] 
        private ViewHolders _viewHolders;
        [SerializeField] 
        private AudioClip _regenerationClip;
        [SerializeField] 
        private AudioClip _pauseClip;
        
        [Inject] 
        private ViewFactory _viewFactory;
        [Inject] 
        private GameStateMachine _gameStateMachine;
        [Inject] 
        private AudioManager _audioManager;
        
        private MainMenu _mainMenu;
        private PauseMenu _pauseMenu;
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        private readonly ICollection<IDisposable> _mainMenuDisposables = new CompositeDisposable();
        private readonly ICollection<IDisposable> _pauseMenuDisposables = new CompositeDisposable();
        
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
            
            return HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left)
                .AppendCallback(() => _gameStateMachine.PlayAsync(NextLevelAsync).Forget());
        }
        
        private async void RestartGameAsync()
        {
            _pauseMenuDisposables.Clear();
            
            UniTask hidePlayerTask = _gameStateMachine.HidePlayerAsync();
            UniTask hidePauseMenuTask = HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();

            await UniTask.WhenAll(hidePlayerTask, hidePauseMenuTask);
            
            _gameStateMachine.RestartLevel();
            await _gameStateMachine.ShowPlayerAsync();
        }

        private async void RegenerateLevelAsync()
        {
            _pauseMenuDisposables.Clear();
            
            UniTask hidePauseMenuTask = HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right).ToUniTask();
            await _gameStateMachine.HidePlayerAsync();
            
            await _gameStateMachine.ExitAsync();
            await hidePauseMenuTask;
            
            _audioManager.PlayOneShot(_regenerationClip);       //TODO Refactor
            
            await _gameStateMachine.PlayAsync(NextLevelAsync);
        }
        
        private async void NextLevelAsync()
        {
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
            
            return HideView(_pauseMenu, _viewHolders.Center, _viewHolders.Right)
                .AppendCallback(() => _gameStateMachine.Continue());
        }
        
        private Sequence NavigateToPauseMenu()
        {
            if (_pauseMenu == null)
            {
                _pauseMenu = _viewFactory.CreateView<PauseMenu>();
                SubscribeToPauseMenuCommands(_pauseMenu, _pauseMenuDisposables);
            }
            
            _audioManager.PlayOneShot(_pauseClip);
            
            return ShowView(_pauseMenu, _viewHolders.Right, _viewHolders.Center);
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
                .Append(HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left))
                .Join(ShowView(settingsMenu, _viewHolders.Right, _viewHolders.Center));
        }

        private Sequence NavigateToMainMenu(MonoBehaviour currentView)
        {
            if (_mainMenu == null)
            {
                _mainMenu = _viewFactory.CreateView<MainMenu>();
                SubscribeToMainMenuCommands(_mainMenu, _mainMenuDisposables);
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
            _mainMenuDisposables.Clear();
            _pauseMenuDisposables.Clear();
            _disposables.Clear();
        }
    }
}