using System.Collections;
using UnityEngine;

public class IceEffect : MonoBehaviour
{
    [Header("Состояния")]
    [SerializeField] private Material defaultMaterial; // обычное
    [SerializeField] private Material effectMaterial; // эффект

    private void OnEnable()
    {
        NilfheimEffect.OnStateChanged += OnEffectChanged;
    }

    private void OnDisable()
    {
        NilfheimEffect.OnStateChanged -= OnEffectChanged;
    }

    private void OnEffectChanged(bool isActive)
    {
        Material target = isActive ? effectMaterial : defaultMaterial;
        gameObject.GetComponent<Renderer>().material = target;
        gameObject.layer = LayerMask.NameToLayer(isActive ? "Ice" : "Default");
    }
}