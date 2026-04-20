using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MidgardEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject globalVolume;
    [SerializeField] private GameObject door;
    public void Apply()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("MidgardEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(true);
        door.SetActive(true);
    }

    public void Remove()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("MidgardEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(false);
        door.SetActive(false);
    }
}