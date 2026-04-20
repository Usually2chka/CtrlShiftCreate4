using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private GameObject explosionVFX;

    [Header("Settings")]
    [SerializeField] private float explosionDuration = 1.5f;
    public float ExplosionDuration => explosionDuration;

    private bool exploded;
    void Awake()
    {
        // гарантируем правильное состояние при спавне
        if (spawnVFX != null)
            spawnVFX.SetActive(true);

        if (explosionVFX != null)
            explosionVFX.SetActive(false);
    }
    public void PlaySpawn()
    {
        exploded = false;

        if (spawnVFX != null)
            spawnVFX.SetActive(true);

        if (explosionVFX != null)
            explosionVFX.SetActive(false);
    }

    public void Explode()
    {
        Debug.Log("Bomb: Explode called");
        if (exploded) return;

        exploded = true;

        if (spawnVFX != null)
            spawnVFX.SetActive(false);

        if (explosionVFX != null)
            explosionVFX.SetActive(true);

        StartCoroutine(DestroyAfter());
    }

    private IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(explosionDuration);

        // просто выключаем, а не уничтожаем
        gameObject.SetActive(false);
    }
}