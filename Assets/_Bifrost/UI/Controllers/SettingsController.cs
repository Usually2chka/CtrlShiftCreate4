using System;
using _Bifrost.UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuUI;
    
    private static VisualElement s_root;
    private Slider _musicSlider;
    private Slider _sensitivitySlider;
    private Button _cancelButton;
    private Button _applyButton;
    
    public void Show()
    {
        s_root.style.display = DisplayStyle.Flex;
    }

    public static void Hide()
    {
        s_root.style.display = DisplayStyle.None;
    }
    
    private void Awake()
    {
        s_root = GetComponent<UIDocument>().rootVisualElement;
        _musicSlider = s_root.Q<Slider>("MusicSlider");
        _sensitivitySlider = s_root.Q<Slider>("SensitivitySlider");
        _cancelButton = s_root.Q<Button>("CancelButton");
        _applyButton = s_root.Q<Button>("ApplyButton");
        
        s_root.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        _musicSlider.value = AudioManager.Instance.GetMusicVolume();

        _musicSlider.RegisterValueChangedCallback(evt =>
        {
            AudioManager.Instance.SetMusicVolume(evt.newValue);
        });
        
        _sensitivitySlider.RegisterValueChangedCallback(evt =>
        {
            PlayerController._mouseSensitivityModifier = evt.newValue;
            Debug.Log($"Sensitivity: {PlayerController._mouseSensitivityModifier}");
        });
        
        _cancelButton.clicked += () =>
        {
            AudioManager.Instance.PlayUISound();
            s_root.style.display = DisplayStyle.None;
            _mainMenuUI.ShowSelf();
        };

        _applyButton.clicked += () =>
        {
            AudioManager.Instance.PlayUISound();
        };
    }

    private void ApplySettings()
    {
        
    }
}
