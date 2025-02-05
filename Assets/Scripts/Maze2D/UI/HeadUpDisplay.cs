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
                    NavigateToMaze();
                    break;
                case MainMenu.CommandKind.Settings:
                    NavigateToSettings(_mainMenu);
                    break;
                case MainMenu.CommandKind.Exit:
                    Application.Quit();
                    break;
            }
        }
        
        private Tween NavigateToMaze()
        {
            return HideView(_mainMenu, _viewHolders.Center, _viewHolders.Left);
        }

        private Tween NavigateToSettings(MonoBehaviour currentView)
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

        private Tween NavigateToMainMenu(MonoBehaviour currentView)
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

        
        private Tween ShowView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            return MoveView(view, from, to)
                .AppendCallback(() =>
                {
                    view.enabled = true;
                });
        }
        
        private Tween HideView(MonoBehaviour view, RectTransform from, RectTransform to)
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