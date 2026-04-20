using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MuspelheimEffect : MonoBehaviour, IWorldEffect
{
    [Header("Spawn")]
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject orbPrefab;

    [Header("VFX")]
    [SerializeField] private GameObject bombPrefab;

    [Header("Settings")]
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
            var instance = CreateOrbInstance(point.position);

            Bomb bomb = null;

            if (bombPrefab != null && instance != null)
            {
                var bombObj = Instantiate(bombPrefab, instance.transform);
                bombObj.transform.localPosition = Vector3.zero;

                bomb = bombObj.GetComponentInChildren<Bomb>();
                bomb?.PlaySpawn();
            }

            orbs.Add(new Orb
            {
                SpawnPoint = point,
                Instance = instance,
                Bomb = bomb,
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
        Debug.LogWarning("MuspelheimEffect: Orb prefab reference is missing.", this);
        return null;
    }

    private void ExplodeOrb(Orb orb)
    {
        if (orb.Instance == null || orb.Exploded) return;

        var explosionPosition = orb.Instance.transform.position;

        orb.Exploded = true;
        orb.Timer = 0f;

        StartCoroutine(ExplodeRoutine(orb, explosionPosition));
    }
    private IEnumerator ExplodeRoutine(Orb orb, Vector3 explosionPosition)
    {
        if (orb.Bomb != null)
        {
            orb.Bomb.Explode();
        }
        else
        {
            Debug.LogWarning("MuspelheimEffect: Bomb component is missing on the orb instance.", this);
        }

        // применяем отталкивание
        if (player != null)
        {
            var direction = (player.transform.position - explosionPosition).normalized;
            if (direction == Vector3.zero)
                direction = Vector3.up;

            player.ApplyKnockback(direction * explosionForce + Vector3.up * (explosionForce * 0.5f));
        }

        yield return new WaitForSeconds(orb.Bomb.ExplosionDuration);
        
        // теперь можно скрыть орб
        if (orb.Instance != null)
            orb.Instance.SetActive(false);
    }
    private void RespawnOrb(Orb orb)
    {
        if (orb.SpawnPoint == null) return;

        if (orb.Instance == null)
        {
            orb.Instance = CreateOrbInstance(orb.SpawnPoint.position);

            // создаём новый VFX
            if (bombPrefab != null)
            {
                var bombObj = Instantiate(bombPrefab, orb.Instance.transform);
                bombObj.transform.localPosition = Vector3.zero;

                orb.Bomb = bombObj.GetComponentInChildren<Bomb>();
            }
        }
        else
        {
            orb.Instance.transform.position = orb.SpawnPoint.position;
            orb.Instance.SetActive(true);

            if (orb.Bomb != null)
            {
                orb.Bomb.gameObject.SetActive(true);
                orb.Bomb.PlaySpawn();
            }
        }
        orb.Exploded = false;
    }

    private void ClearOrbs()
    {
        foreach (var orb in orbs)
        {
            if (orb.Instance != null)
                Destroy(orb.Instance);
        }

        orbs.Clear();
    }

    private void OnDisable()
    {
        ClearOrbs();
    }
}