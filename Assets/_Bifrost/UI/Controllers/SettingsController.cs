using System;
using _Bifrost.UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuUI;
    
    private VisualElement _root;
    private Slider _musicSlider;
    private Slider _sensitivitySlider;
    private Button _cancelButton;
    private Button _applyButton;

    public void Show()
    {
        _root.style.display = DisplayStyle.Flex;
    }
    
    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _musicSlider = _root.Q<Slider>("MusicSlider");
        _sensitivitySlider = _root.Q<Slider>("SensitivitySlider");
        _cancelButton = _root.Q<Button>("CancelButton");
        _applyButton = _root.Q<Button>("ApplyButton");
        
        _root.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        _musicSlider.value = AudioManager.Instance.GetMusicVolume();

        _musicSlider.RegisterValueChangedCallback(evt =>
        {
            AudioManager.Instance.SetMusicVolume(evt.newValue);
        });
        
        _cancelButton.clicked += () =>
        {
            _root.style.display = DisplayStyle.None;
            _mainMenuUI.ShowSelf();
        };
    }

    private void ApplySettings()
    {
        
    }
}
