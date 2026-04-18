using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VanaheimEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private GameObject spawnRoot;
    [SerializeField] private LianaSpawner[] spawnPoints;
    [SerializeField] private GameObject lianaPrefab;

    private bool isActive;
    private readonly List<Liana> lianas = new List<Liana>();

    public void Apply()
    {
        if (isActive) return;

        if ((spawnPoints == null || spawnPoints.Length == 0) && spawnRoot != null)
        {
            spawnPoints = spawnRoot.GetComponentsInChildren<LianaSpawner>();
        }

        CreateLianas();
        isActive = true;
    }

    public void Remove()
    {
        isActive = false;
        ClearLianas();
    }
    private void CreateLianas()
    {
        ClearLianas();

        foreach (var spawner in spawnPoints)
        {
            if (spawner == null) continue;

            var instance = CreateLianaInstance(spawner);
            
            lianas.Add(new Liana
            {
                SpawnPoint = spawner.transform,
                Instance = instance
            });
        }
    }
    private GameObject CreateLianaInstance(LianaSpawner spawner)
    {
        if (lianaPrefab == null)
        {
            Debug.LogError("Liana prefab is not assigned!", this);
            return null;
        }

        var obj = Instantiate(
            lianaPrefab,
            spawner.Position,
            spawner.Rotation,
            transform
        );

        obj.transform.localScale = spawner.Scale;

        return obj;
    }

    private void ClearLianas()
    {
        foreach (var liana in lianas)
        {
            if (liana.Instance != null)
            {
                Destroy(liana.Instance);
            }
        }

        lianas.Clear();
    }

    private void OnDisable()
    {
        ClearLianas();
    }
}
