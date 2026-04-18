using UnityEngine;
using _Bifrost.Runtime.Managers.GamePlay;
using _Bifrost.Runtime.Portals;

[RequireComponent(typeof(Outline))]
public class Crystal : MonoBehaviour, IInventoryItem
{
    [SerializeField] private WorldType type;
    [SerializeField] private Texture2D _icon;
    [SerializeField] private Texture2D _inactiveIcon; // спрайт в неактивном состоянии
    [SerializeField] private Material _inactiveMaterial; // материал в неактивном состоянии
    [SerializeField] private Material _originalMaterial; // связанный портал (для обновления стабильности при вставке в сокет)

    public Texture2D Icon => IsActive ? _icon : (_inactiveIcon != null ? _inactiveIcon : _icon);
    public WorldType CrystalType => type;
    public bool IsActive { get; private set; } = false;

    protected Outline _outline;
    private Renderer _renderer;

    private void OnEnable()
    {
        _outline = GetComponent<Outline>();
        _outline.OutlineWidth = 0;
        _renderer = GetComponent<Renderer>();
        _renderer.material = IsActive ? _originalMaterial : _inactiveMaterial;
    }

    public void SetActive(bool state)
    {
        IsActive = state;
        
        // Обновляем материал объекта
        if (_renderer != null)
        {
            if (state)
            {
                _renderer.material = _originalMaterial;
            }
            else if (_inactiveMaterial != null)
            {
                _renderer.material = _inactiveMaterial;
            }
        }
    }

    public void Hide()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }

    public void Show()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
        }
                // Обновляем материал объекта
        if (_renderer != null)
        {
            if (IsActive)
            {
                _renderer.material = _originalMaterial;
            }
            else if (_inactiveMaterial != null)
            {
                _renderer.material = _inactiveMaterial;
            }
        }
    }

    public void OnHoverEnter()
    {
        _outline.OutlineWidth = 5;
    }

    public void OnHoverExit()
    {
        _outline.OutlineWidth = 0;
    }
}