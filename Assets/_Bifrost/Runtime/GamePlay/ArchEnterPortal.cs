using _Bifrost.Runtime.Portals;
using UnityEngine;

namespace _Bifrost.Runtime.Managers.GamePlay
{
    public class ArchEnterPortal : InteractiveObject
    {
        [SerializeField] private Portal _portal; // портал, который открывает/закрывает
        
        public Portal Portal => _portal;
        
        public void Interact()
        {
            if (_portal == null) return;
            
            if (_portal.state == PortalState.Closed)
            {
                OpenPortal();
            }
            else
            {
                ClosePortal();
            }
        }
        
        private void OpenPortal()
        {
            _portal.Open();
        }
        
        private void ClosePortal()
        {
            _portal.Close();
        }
    }
}