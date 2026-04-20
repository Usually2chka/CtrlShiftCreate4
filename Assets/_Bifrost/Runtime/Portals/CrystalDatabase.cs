using UnityEngine;
using _Bifrost.Runtime.Portals;

[CreateAssetMenu(menuName = "ScriptableObjects/Crystal Database")]
public class CrystalDatabase : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public WorldType type;
        public Crystal prefab;
    }

    public Entry[] crystals;

    public Crystal GetPrefab(WorldType type)
    {
        foreach (var c in crystals)
        {
            if (c.type == type)
                return c.prefab;
        }

        Debug.LogWarning($"Нет префаба для типа {type}");
        return null;
    }
}