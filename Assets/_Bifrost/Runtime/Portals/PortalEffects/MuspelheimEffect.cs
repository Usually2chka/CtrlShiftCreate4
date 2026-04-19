using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuspelheimEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject orbPrefab;
    [SerializeField] private float proximityDistance = 2f;
    [SerializeField] private float explosionForce = 12f;
    [SerializeField] private float respawnTime = 5f;

    private PlayerController player;
    private bool isActive;
    private readonly List<Orb> orbs = new List<Orb>();

    public void Apply()
    {
        if (isActive) return;

        player = GameManager.Instance.Player;

        if ((spawnPoints == null || spawnPoints.Length == 0) && spawnRoot != null)
        {
            spawnPoints = spawnRoot.Cast<Transform>().ToArray();
        }


        CreateOrbs();
        isActive = true;
    }

    public void Remove()
    {
        isActive = false;
        ClearOrbs();
    }

    private void Update()
    {
        if (!isActive || orbs.Count == 0 || player == null) return;

        foreach (var orb in orbs)
        {
            if (orb.Exploded)
            {
                orb.Timer += Time.deltaTime;
                if (orb.Timer >= respawnTime)
                {
                    RespawnOrb(orb);
                }
                continue;
            }

            if (orb.Instance == null) continue;

            var distance = Vector3.Distance(player.transform.position, orb.Instance.transform.position);
            if (distance <= proximityDistance)
            {
                ExplodeOrb(orb);
            }
        }
    }

    private void CreateOrbs()
    {
        ClearOrbs();

        foreach (var point in spawnPoints)
        {
            if (point == null) continue;
            var instance = CreateOrbInstance(point.position);
            orbs.Add(new Orb
            {
                SpawnPoint = point,
                Instance = instance,
                Exploded = false,
                Timer = 0f
            });
        }
    }

    private GameObject CreateOrbInstance(Vector3 position)
    {
        if (orbPrefab != null)
        {
            return Instantiate(orbPrefab, position, Quaternion.identity, transform);
        }

        var fallback = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        fallback.transform.SetParent(transform);
        fallback.transform.position = position;
        fallback.transform.localScale = Vector3.one * 0.5f;
        fallback.name = "MuspelheimOrb";

        var collider = fallback.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        var renderer = fallback.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        return fallback;
    }

    private void ExplodeOrb(Orb orb)
    {
        if (orb.Instance == null || orb.Exploded) return;

        var explosionPosition = orb.Instance.transform.position;
        orb.Instance.SetActive(false);
        orb.Exploded = true;
        orb.Timer = 0f;

        if (player != null)
        {
            var direction = (player.transform.position - explosionPosition).normalized;
            if (direction == Vector3.zero)
            {
                direction = Vector3.up;
            }
            player.ApplyKnockback(direction * explosionForce + Vector3.up * (explosionForce * 0.5f));
        }
    }

    private void RespawnOrb(Orb orb)
    {
        if (orb.SpawnPoint == null) return;

        if (orb.Instance == null)
        {
            orb.Instance = CreateOrbInstance(orb.SpawnPoint.position);
        }
        else
        {
            orb.Instance.transform.position = orb.SpawnPoint.position;
            orb.Instance.SetActive(true);
        }

        orb.Exploded = false;
        orb.Timer = 0f;
    }

    private void ClearOrbs()
    {
        foreach (var orb in orbs)
        {
            if (orb.Instance != null)
            {
                Destroy(orb.Instance);
            }
        }

        orbs.Clear();
    }

    private void OnDisable()
    {
        ClearOrbs();
    }
}
