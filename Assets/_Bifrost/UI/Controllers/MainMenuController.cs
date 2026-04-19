using _Bifrost.Runtime.Managers;

namespace _Bifrost.UI.Controllers
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private SettingsController _settingsController;
        [SerializeField] private HUDController _hudController;
        
        private VisualElement _root;
        private Button _playButton;
        private Button _settingsButton;
        private Button _exitButton;
        
        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            _playButton = _root.Q<Button>("StartGameButton");
            _settingsButton = _root.Q<Button>("SettingsButton");
            _exitButton = _root.Q<Button>("ExitButton");
        }
        
        private void Start()
        {
            _playButton.clicked += () =>
            {
                GameManager.Instance.SetState(GameState.RUNNING);
                _hudController.Show();
                AudioManager.Instance.PlayUISound();
                _root.style.display = DisplayStyle.None;
            };
            
            _settingsButton.clicked += () =>
            {
                _settingsController.Show();
                AudioManager.Instance.PlayUISound();
                _root.style.display = DisplayStyle.None;
            };

            _exitButton.clicked += () =>
            {
                AudioManager.Instance.PlayUISound();
                Application.Quit();
            };
        }

        public void ShowSelf()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void HideSelf()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}