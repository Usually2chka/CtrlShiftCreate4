using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    private VisualElement _root;
    private Slider _musicSlider;
    private Button _cancelButton;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _musicSlider = _root.Q<Slider>("MusicSlider");
        _cancelButton = _root.Q<Button>("CancelButton");

        _cancelButton.clicked += () =>
        {
            GameManager.Instance.ShowMainMenu();
        };
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
    }
}
