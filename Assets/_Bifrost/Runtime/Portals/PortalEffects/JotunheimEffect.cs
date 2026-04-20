using UnityEngine;

public class JotunheimEffect : MonoBehaviour, IWorldEffect
{
    public void Apply()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.06f;
        RenderSettings.fogColor = new Color(0.37f, 0.56f, 0.67f);
    }

    public void Remove()
    {
        RenderSettings.fog = false;
    }
}