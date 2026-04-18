using System;
using _Bifrost.UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuUI;
    
    private VisualElement _root;
    private Slider _musicSlider;
    private Button _cancelButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _musicSlider = _root.Q<Slider>("MusicSlider");
        _cancelButton = _root.Q<Button>("CancelButton");

        _root.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        // установить текущее значение
        _musicSlider.value = AudioManager.Instance.GetMusicVolume();

        // подписка на изменение
        _musicSlider.RegisterValueChangedCallback(evt =>
        {
            AudioManager.Instance.SetMusicVolume(evt.newValue);
        });
        
        _cancelButton.clicked += () =>
        {
            _root.style.display = DisplayStyle.None;
            _mainMenuUI.Show();
        };
    }

    public void Show()
    {
        _root.style.display = DisplayStyle.Flex;
    }
}
