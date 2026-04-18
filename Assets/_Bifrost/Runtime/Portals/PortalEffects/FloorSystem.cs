using System;

public static class FloorSystem
{
    public static bool IsActive = false;

    public static event Action<bool> OnStateChanged;

    public static void SetState(bool state)
    {
        IsActive = state;
        OnStateChanged?.Invoke(state);
    }

    public static void Toggle()
    {
        SetState(!IsActive);
    }

    public static void SetActive(bool active)
    {
        SetState(active);
    }
}