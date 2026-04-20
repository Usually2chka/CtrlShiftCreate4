using UnityEngine;
using System;

public class SvartalfheimEffect  : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject defaultStair;
    [SerializeField] private GameObject effectStair;

    public static bool IsActive { get; private set; }

    public static event Action<bool> OnStateChanged;

    public static void SetState(bool state)
    {
        IsActive = state;
        OnStateChanged?.Invoke(state);
    }
    

    public void Apply()
    {
        SetState(true);
        effectStair.SetActive(true);
        defaultStair.SetActive(false);
    }

    public void Remove()
    {
        SetState(false);
        effectStair.SetActive(false);
        defaultStair.SetActive(true);
    }
}