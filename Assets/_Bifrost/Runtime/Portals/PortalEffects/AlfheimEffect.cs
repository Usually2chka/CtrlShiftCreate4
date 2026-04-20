using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlfheimEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject globalVolume;
    [SerializeField] private GameObject rootWorldObjectsToDisable;
    [SerializeField] private GameObject portalHart;
    [SerializeField] private Renderer treeRenderer;
    [SerializeField] private float effectDuration = 1f;
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
        if (portalHart != null)
        {
            Renderer renderer = portalHart.GetComponent<Renderer>();
            renderer.material.SetFloat("_Flex", 100f);
        }
        if (treeRenderer != null)
        {
            treeRenderer.material.SetFloat("_Flex", effectDuration);
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
        Debug.Log("AlfheimEffect: Removing effect, deactivating global volume.");
        if (rootWorldObjectsToDisable != null)
        {
            Debug.Log("AlfheimEffect: Removing effect, resetting _Flex for all children of " + rootWorldObjectsToDisable.name);
            foreach (Transform child in rootWorldObjectsToDisable.transform)
            {
                Debug.Log("AlfheimEffect: Resetting _Flex for child " + child.name);
                foreach (Transform ch in child)
                {
                    Debug.Log("AlfheimEffect: Resetting _Flex for " + ch.name);
                    Renderer renderer = ch.GetComponent<Renderer>();
                    renderer.material.SetFloat("_Flex", 0f);
                }
            }
        }
        if (portalHart != null)
        {
            Renderer renderer = portalHart.GetComponent<Renderer>();
            renderer.material.SetFloat("_Flex", 0f);
        }
        if (treeRenderer != null)
        {
            treeRenderer.material.SetFloat("_Flex", 0f);
        }
    }
}