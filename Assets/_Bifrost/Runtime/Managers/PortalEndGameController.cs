using System.Linq;
using _Bifrost.Runtime.Portals;
using UnityEngine;

namespace _Bifrost.Runtime.Managers
{
    public class PortalEndGameController : MonoBehaviour
    {
        private Portal[] portals;
        private bool hasTriggeredEndGame;

        private void Start()
        {
            RefreshPortals();
            UpdateEndGameStatus();
        }

        private void OnDestroy()
        {
            UnsubscribePortalEvents();
        }

        private void RefreshPortals()
        {
            UnsubscribePortalEvents();
            portals = FindObjectsOfType<Portal>();
            foreach (var portal in portals)
            {
                portal.OnStateChanged += OnPortalStateChanged;
            }
        }

        private void UnsubscribePortalEvents()
        {
            if (portals == null) return;

            foreach (var portal in portals)
            {
                if (portal != null)
                {
                    portal.OnStateChanged -= OnPortalStateChanged;
                }
            }
        }

        private void OnPortalStateChanged(PortalState newState)
        {
            UpdateEndGameStatus();
        }

        private void UpdateEndGameStatus()
        {
            if (hasTriggeredEndGame)
                return;

            portals = FindObjectsOfType<Portal>();
            if (portals.Length == 0)
                return;

            bool allStabilized = portals.All(p => p.state == PortalState.Stabilized);
            if (allStabilized && GameManager.Instance != null)
            {
                hasTriggeredEndGame = true;
                GameManager.Instance.EndGame();
            }
        }
    }
}