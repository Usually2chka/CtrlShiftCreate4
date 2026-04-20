using System.Collections.Generic;
using System.Linq;
using _Bifrost.Runtime.Portals;
using UnityEngine;

public class VanaheimEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject Lians;
    public void Apply()
    {
        Lians.SetActive(true);
    }

    public void Remove()
    {
        Lians.SetActive(false);
    }
}
