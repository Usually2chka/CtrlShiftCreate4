using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlfheimEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject globalVolume;
    [SerializeField] private GameObject rootWorldObjectsToDisable;
    public void Apply()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("AlfheimEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(true);
        if (rootWorldObjectsToDisable != null)
        {
            foreach (Transform child in rootWorldObjectsToDisable.transform)
            {
                foreach (Transform ch in child)
                {
                    Renderer renderer = ch.GetComponent<Renderer>();
                    renderer.material.SetFloat("_Flex", 2f);
                }
            }
        }
    }

    public void Remove()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("AlfheimEffect: Global Volume reference is missing.", this);
            return;
        }
        globalVolume.SetActive(false);
        if (rootWorldObjectsToDisable != null)
        {
            foreach (Transform child in rootWorldObjectsToDisable.transform)
            {
                foreach (Transform ch in child)
                {
                    Renderer renderer = ch.GetComponent<Renderer>();
                    renderer.material.SetFloat("_Flex", 2f);
                }
            }
        }
    }
}