using System;
using UnityEngine;
using UnityEngine.UIElements;

public class NoteController : MonoBehaviour
{
    public event Action OnClosed;
    
    [SerializeField] private Texture2D noteTexture;
    private VisualElement note;
    private Button _closeNoteButton;
    private Image noteImage;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        note = root.Q<VisualElement>("noteContainer");
        _closeNoteButton = root.Q<Button>("CloseNoteButton");
        noteImage = root.Q<Image>("NoteImage");


        noteImage.image = noteTexture;

        note.style.display = DisplayStyle.None;
        _closeNoteButton.clicked += OnCloseNoteClicked;
    }

    public void ShowNote()
    {
        note.style.display = DisplayStyle.Flex;
    }

    public void HideNote()
    {
        note.style.display = DisplayStyle.None;
    }
    
    private void OnCloseNoteClicked()
    {
        HideNote();

        OnClosed?.Invoke();

        Destroy(gameObject);
    }
}
