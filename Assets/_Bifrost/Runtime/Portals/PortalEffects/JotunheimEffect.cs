using UnityEngine;

public class JotunheimEffect : MonoBehaviour, IWorldEffect
{
    public void Apply()
    {
        RenderSettings.fog = true;
    }

    public void Remove()
    {
        RenderSettings.fog = false;
    }
}