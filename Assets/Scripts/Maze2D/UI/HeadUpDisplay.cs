using System;
using System.Collections.Generic;
using DG.Tweening;
using Maze2D.CodeBase.View;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace Maze2D.UI
{
    public class HeadUpDisplay : MonoBehaviour
    {
        [Inject] private ViewFactory _viewFactory;

        [SerializeField] private ViewHolders _viewHolders = default;

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
            _mainMenu.CommandInvoked.AsObservable().Subscribe(command =>
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

            }).AddTo(_disposables);
        }

        private Tween NavigateToSettings(MonoBehaviour currentView)
        {
            const float navigationDuration = 1.5f;

            currentView.enabled = false;
            
            SettingsMenu settingsMenu = _viewFactory.CreateView<SettingsMenu>();
            settingsMenu.transform.localPosition = _viewHolders.Right.localPosition;
            settingsMenu.enabled = false;
            
            return DOTween.Sequence()
                .Append(currentView.transform
                    .DOLocalMoveX(_viewHolders.Left.localPosition.x, navigationDuration)
                    .SetEase(Ease.OutSine))
                .Join(settingsMenu.transform
                    .DOLocalMoveX(_viewHolders.Center.localPosition.x, navigationDuration)
                    .SetEase(Ease.OutSine))
                .AppendCallback(() =>
                {
                    settingsMenu.enabled = true;
                    currentView.gameObject.SetActive(false);
                })
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private Tween NavigateToMainMenu(MonoBehaviour currentView)
        {
            const float navigationDuration = 1.5f;
            
            return DOTween.Sequence()
                .Append(currentView.transform
                    .DOLocalMoveX(_viewHolders.Right.localPosition.x, navigationDuration)
                    .SetEase(Ease.OutSine))
                .Join(_mainMenu.transform
                    .DOLocalMoveX(_viewHolders.Center.localPosition.x, navigationDuration)
                    .SetEase(Ease.OutSine))
                .AppendCallback(() => Destroy(currentView.gameObject))
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}