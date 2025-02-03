using System.Collections.Generic;
using UnityEngine;

namespace Maze2D.CodeBase.View
{
    [CreateAssetMenu(fileName = "view_container", menuName = "Containers/ViewContainer", order = 0)]
    public class ViewContainer : ScriptableObject
    {
        [SerializeField] private List<MonoBehaviour> _viewPrefabs;

        public TView GetViewPrefab<TView>() where TView : MonoBehaviour
        {
            foreach (MonoBehaviour viewPrefab in _viewPrefabs)
            {
                if (viewPrefab is TView concreteViewPrefab)
                {
                    return concreteViewPrefab;
                }
            }

            return null;
        }
    }
}