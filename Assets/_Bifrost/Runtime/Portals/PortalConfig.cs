using System.Collections.Generic;
using UnityEngine;

namespace _Bifrost.Runtime.Portals
{
    [CreateAssetMenu(fileName = "PortalConfig", menuName = "ScriptableObjects/PortalConfig")]
    public class PortalConfig : ScriptableObject
{
    public string portalId;
    public WorldType worldType;

    public List<CrystalRequirement> stabilizationCost;
}

[System.Serializable]
public struct CrystalRequirement
{
    public WorldType type;
    public int cost;
}

public enum PortalState
{
    Closed,
    OpenUnstable,
    Stabilized
}

public enum WorldType
{
    Asgard,
    Midgard,
    Jotunheim,
    Vanaheim,
    Alfheim,
    Svartalfheim,
    Helheim,
    Muspelheim,
    Niflheim
}
}