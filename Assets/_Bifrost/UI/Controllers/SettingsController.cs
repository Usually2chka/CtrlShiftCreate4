using System;
using _Bifrost.UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuUI;
    
    private static VisualElement s_root;
    private Slider _musicSlider;
    private Slider _musicSFXSlider;
    private Slider _sensitivitySlider;
    private Button _okayButton;
    
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
        _musicSFXSlider = s_root.Q<Slider>("MusicSFXSlider");
        _sensitivitySlider = s_root.Q<Slider>("SensitivitySlider");
        _okayButton = s_root.Q<Button>("OkayButton");
        
        s_root.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        _musicSlider.value = AudioManager.Instance.GetMusicVolume();

        _musicSlider.RegisterValueChangedCallback(evt =>
        {
            AudioManager.Instance.SetMusicVolume(evt.newValue);
        });

        _musicSFXSlider.RegisterValueChangedCallback(evt =>
        {
            AudioManager.Instance.SetMusicSFXVolume(evt.newValue);
        });
        
        _sensitivitySlider.RegisterValueChangedCallback(evt =>
        {
            PlayerController._mouseSensitivityModifier = evt.newValue;
        });
        
        _okayButton.clicked += () =>
        {
            AudioManager.Instance.PlayUISound();
            s_root.style.display = DisplayStyle.None;
            _mainMenuUI.ShowSelf();
        };
    }
}
