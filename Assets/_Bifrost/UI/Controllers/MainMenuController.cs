using _Bifrost.Runtime.Managers;

namespace _Bifrost.UI.Controllers
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MainMenuController : MonoBehaviour
    {
        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            
            var playButton = root.Q<Button>("StartGameButton");
            var settingsButton = root.Q<Button>("SettingsButton");

            playButton.clicked += () =>
            {
                Debug.Log("Play");
                GameManager.Instance.SetState(GameState.RUNNING);
            };
            
            settingsButton.clicked += () =>
            {
                GameManager.Instance.ShowSettings();
            };
        }
    }
}