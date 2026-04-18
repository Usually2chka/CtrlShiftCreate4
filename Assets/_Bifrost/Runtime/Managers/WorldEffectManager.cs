using System.Collections.Generic;
using UnityEngine;

public class WorldEffectManager : MonoBehaviour
{
    public static WorldEffectManager Instance { get; private set; }
    private List<IWorldEffect> activeEffects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void RegisterEffect(IWorldEffect effect)
    {
        activeEffects.Add(effect);
        effect.Apply();
    }

    public void RemoveEffect(IWorldEffect effect)
    {
        effect.Remove();
        activeEffects.Remove(effect);
    }
}

public interface IWorldEffect
{
    void Apply();
    void Remove();
}
