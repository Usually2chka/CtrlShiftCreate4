using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MidgardEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject globalVolume;
    [SerializeField] private Renderer doorRenderer;
    private Material _originalDoorMaterial;
    [SerializeField] private Material _invisibleDoorMaterial;
    void Start()
    {
        if (doorRenderer != null) {
            _originalDoorMaterial = doorRenderer.material;
        }
    }
    public void Apply()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("MidgardEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(true);
        if (doorRenderer != null && _invisibleDoorMaterial != null)
        {
            doorRenderer.material = _invisibleDoorMaterial;
        }
    }

    public void Remove()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("MidgardEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(false);
        if (doorRenderer != null && _originalDoorMaterial != null)
        {
            doorRenderer.material = _originalDoorMaterial;
        }
    }
}