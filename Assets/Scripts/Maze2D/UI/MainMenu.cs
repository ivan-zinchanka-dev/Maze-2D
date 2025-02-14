using System;
using System.Collections.Generic;
using Maze2D.CodeBase.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
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
        
        [field: SerializeField] 
        public UnityEvent<CommandKind> CommandInvoked { get; private set; }
        
        private readonly ICollection<IDisposable> _disposables = new CompositeDisposable();

        public enum CommandKind
        {
            None = 0,
            Play = 1,
            Settings = 2,
            Exit = 3,
        }

        private void OnEnable()
        {
            InvokeCommandOnClick(_playButton, CommandKind.Play);
            InvokeCommandOnClick(_settingsButton, CommandKind.Settings);
            InvokeCommandOnClick(_exitButton, CommandKind.Exit);
            
            SelectDefaultObject();
        }

        private void SelectDefaultObject()
        {
            EventSystem.current.SetSelectedGameObject(_playButton.gameObject);
        }
        
        private void InvokeCommandOnClick(Button button, CommandKind commandKind)
        {
            button
                .OnClickAsObservable()
                .Subscribe(unit => CommandInvoked.Invoke(commandKind))
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Clear();
            EventSystem.current.Release();
        }
    }
}