using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlfheimEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject globalVolume;
    public void Apply()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("AlfheimEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(true);
    }

    public void Remove()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("AlfheimEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(false);
    }
}