using UnityEngine;

namespace _Bifrost.Runtime.Managers.GamePlay
{
    [RequireComponent(typeof(Outline))]
    public class InteractiveObject : MonoBehaviour, IInventoryItem
    {
        [SerializeField] private Texture2D _icon;
        
        public Texture2D Icon => _icon;
        
        protected Outline _outline;

        private void OnEnable()
        {
            _outline = GetComponent<Outline>();
            _outline.OutlineWidth = 0;
        }

        public void OnHoverEnter()
        {
            _outline.OutlineWidth = 5;
        }

        public void OnHoverExit()
        {
            _outline.OutlineWidth = 0;
        }
    }
}