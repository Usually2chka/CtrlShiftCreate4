using UnityEngine;
using System;

public class NilfheimEffect : MonoBehaviour, IWorldEffect
{
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
    }

    public void Remove()
    {
        SetState(false);
    }
}