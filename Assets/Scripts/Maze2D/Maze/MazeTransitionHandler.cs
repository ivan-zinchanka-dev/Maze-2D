using DG.Tweening;
using UnityEngine;

namespace Maze2D.Maze
{
    public class MazeTransitionHandler
    {
        private const float TransitionDuration = 1.0f;
        private readonly Transform _maze;
        
        public MazeTransitionHandler(Transform maze)
        {
            _maze = maze;
        }
        
        public Tween Show()
        {
            _maze.localScale = Vector3.zero;
            _maze.localEulerAngles = new Vector3(0f, 0f, -90f);

            return DOTween.Sequence()
                .Append(_maze.DOScale(Vector3.one, TransitionDuration).SetEase(Ease.OutCirc))
                .Join(_maze.DOLocalRotate(Vector3.zero, TransitionDuration).SetEase(Ease.OutCirc))
                .SetLink(_maze.gameObject);
        }

        public Tween Hide()
        {
            _maze.localScale = Vector3.one;
            _maze.localEulerAngles = Vector3.zero;
            
            return DOTween.Sequence()
                .Append(_maze.DOScale(Vector3.zero, TransitionDuration).SetEase(Ease.OutCirc))
                .Join(_maze.DOLocalRotate(new Vector3(0f, 0f, 90f), TransitionDuration).SetEase(Ease.OutCirc))
                .SetLink(_maze.gameObject);
        }
    }
}