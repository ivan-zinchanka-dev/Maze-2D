using DG.Tweening;
using UnityEngine;

namespace Maze2D.UI
{
    public class ViewNavigationService
    {
        private readonly float _navigationDuration;
        
        public ViewNavigationService(float navigationDuration)
        {
            _navigationDuration = navigationDuration;
        }

        public Sequence ShowView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            return MoveView(view, from, to)
                .AppendCallback(() =>
                {
                    view.enabled = true;
                });
        }
        
        public Sequence HideView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            return MoveView(view, from, to)
                .AppendCallback(() =>
                {
                    Object.Destroy(view.gameObject);
                });
        }

        private Sequence MoveView(MonoBehaviour view, RectTransform from, RectTransform to)
        {
            view.enabled = false;
            view.transform.localPosition = from.localPosition;
            
            return DOTween.Sequence()
                .Append(view.transform
                    .DOLocalMoveX(to.localPosition.x, _navigationDuration)
                    .SetEase(Ease.OutSine))
                .SetUpdate(true)
                .SetLink(view.gameObject);
        }
    }
}