using UnityEngine;
public class Portal : MonoBehaviour
{
    public PortalConfig config;
    public PortalState state;

    private IWorldEffect worldEffect;

    private void Awake()
    {
        worldEffect = GetComponent<IWorldEffect>();
        if (worldEffect == null)
    {
        Debug.LogError("IWorldEffect component is missing!", this);
    }
    }

    public void Open()
    {
        if (state != PortalState.Closed) return;
        if (WorldEffectManager.Instance == null)
{
    Debug.LogError("WorldEffectManager Instance is NULL!");
    return;
}

        state = PortalState.OpenUnstable;
        WorldEffectManager.Instance.RegisterEffect(worldEffect);
        ActivatePortal(true);
    }

    public void Close()
    {
        state = PortalState.Closed;
        WorldEffectManager.Instance.RemoveEffect(worldEffect);
        ActivatePortal(false);
    }

    public void Stabilize()
    {
        if (state != PortalState.OpenUnstable) return;

        state = PortalState.Stabilized;
        WorldEffectManager.Instance.RemoveEffect(worldEffect);
    }

    public void Destabilize()
    {
        if (state != PortalState.Stabilized) return;

        state = PortalState.OpenUnstable;
        WorldEffectManager.Instance.RegisterEffect(worldEffect);
    }

    private void ActivatePortal(bool active)
    {
        // визуал / интеракция
    }
}