using System;
using Maze2D.CodeBase.Controls;
using Maze2D.Controls;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VContainer;

namespace Maze2D.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] 
        private Button _restartLevelButton;
        [SerializeField] 
        private Button _newLevelButton;
        [SerializeField] 
        private Button _mainMenuButton;
        
        [field: SerializeField] 
        public UnityAction<CommandKind> CommandInvoked { get; private set; }

        [Inject] 
        private IInputSystemService _inputSystemService;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public enum CommandKind
        {
            None = 0,
            RestartLevel = 1,
            RegenerateLevel = 2,
            ToMainMenu = 3,
            Continue = 4,
        }

        private void OnEnable()
        {
            InvokeCommandOnClick(_restartLevelButton, CommandKind.RestartLevel);
            InvokeCommandOnClick(_newLevelButton, CommandKind.RegenerateLevel);
            InvokeCommandOnClick(_mainMenuButton, CommandKind.ToMainMenu);
            
            Observable.EveryUpdate()
                .Where(ResumeGameDemand)
                .Subscribe(unit => CommandInvoked.Invoke(CommandKind.Continue))
                .AddTo(_disposables);
        }
        
        private void InvokeCommandOnClick(Button button, CommandKind commandKind)
        {
            button
                .OnClickAsObservable()
                .Subscribe(unit => CommandInvoked.Invoke(commandKind))
                .AddTo(_disposables);
        }

        private bool ResumeGameDemand(long unit)
        {
            return _inputSystemService.GetButtonUp(InputActions.Pause);
        }

        private void OnDisable()
        {
            _disposables.Clear();
        }
    }
}