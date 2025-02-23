using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Maze2D.View
{
    internal class ViewFactory : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform _viewParent;
        [SerializeField] 
        private ViewContainer _viewContainer;
        [Inject] 
        private IObjectResolver _diContainer;
        
        public TView CreateView<TView>(Transform parent = null) where TView : MonoBehaviour
        {
            TView viewPrefab = _viewContainer.GetViewPrefab<TView>();

            if (viewPrefab != null)
            {
                if (parent != null)
                {
                    return _diContainer.Instantiate<TView>(viewPrefab, parent);
                }
                else
                {
                    return _diContainer.Instantiate<TView>(viewPrefab, _viewParent);
                }
            }
            else
            {
                return null;
            }
        }
    }
}