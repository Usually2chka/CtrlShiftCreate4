using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HelheimEffect : MonoBehaviour, IWorldEffect
{
    public void Apply()
    {
        FloorSystem.SetActive(true);
    }

    public void Remove()
    {
        FloorSystem.SetActive(false);
    }
}