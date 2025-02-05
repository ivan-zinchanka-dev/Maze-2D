using System;
using System.Collections.Generic;
using DG.Tweening;
using Maze2D.CodeBase.View;
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
        private ViewHolders _viewHolders = default;

        [Inject] 
        private ViewFactory _viewFactory;
        
        [Serializable]
        private struct ViewHolders {

            public RectTransform Left;
            public RectTransform Center;
            public RectTransform Right;
        }

        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();
        
        private MainMenu _mainMenu;
        private SettingsMenu _settingsMenu;
        
        //private MonoBehaviour _currentView;
        
        private void Awake()
        {
            _mainMenu = _viewFactory.CreateView<MainMenu>();
        }

        private void OnEnable()
        {
            _mainMenu.CommandInvoked.AddListener(ProcessMainMenuCommand);
        }

        private void ProcessMainMenuCommand(MainMenu.CommandKind command)
        {
            switch (command)
            {
                case MainMenu.CommandKind.Play:
                    break;
                case MainMenu.CommandKind.Settings:
                    NavigateToSettings(_mainMenu);
                    break;
                case MainMenu.CommandKind.Exit:
                    Application.Quit();
                    break;
            }
        }

        private Tween NavigateToSettings(MonoBehaviour currentView)
        {
            currentView.enabled = false;
            
            SettingsMenu settingsMenu = _viewFactory.CreateView<SettingsMenu>();
            settingsMenu.transform.localPosition = _viewHolders.Right.localPosition;
            settingsMenu.enabled = false;
            
            settingsMenu.OnBack.AddListener(() =>
            {
                NavigateToMainMenu(settingsMenu);
            });
            
            return DOTween.Sequence()
                .Append(currentView.transform
                    .DOLocalMoveX(_viewHolders.Left.localPosition.x, _viewNavigationDuration)
                    .SetEase(Ease.OutSine))
                .Join(settingsMenu.transform
                    .DOLocalMoveX(_viewHolders.Center.localPosition.x, _viewNavigationDuration)
                    .SetEase(Ease.OutSine))
                .AppendCallback(() =>
                {
                    settingsMenu.enabled = true;
                    Destroy(currentView.gameObject);
                })
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private Tween NavigateToMainMenu(MonoBehaviour currentView)
        {
            currentView.enabled = false;

            if (_mainMenu == null)
            {
                _mainMenu = _viewFactory.CreateView<MainMenu>();
                _mainMenu.transform.localPosition = _viewHolders.Left.localPosition;
                _mainMenu.CommandInvoked.AddListener(ProcessMainMenuCommand);
                _mainMenu.enabled = false;
            }

            return DOTween.Sequence()
                .Append(currentView.transform
                    .DOLocalMoveX(_viewHolders.Right.localPosition.x, _viewNavigationDuration)
                    .SetEase(Ease.OutSine))
                .Join(_mainMenu.transform
                    .DOLocalMoveX(_viewHolders.Center.localPosition.x, _viewNavigationDuration)
                    .SetEase(Ease.OutSine))
                .AppendCallback(() =>
                {
                    _mainMenu.enabled = true;
                    Destroy(currentView.gameObject);
                })
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}