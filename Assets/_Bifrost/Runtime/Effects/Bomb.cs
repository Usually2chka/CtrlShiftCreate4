using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("VFX")]
    public GameObject spawnVFX;
    public GameObject triggerVFX;

    [Header("Settings")]
    public float activationDelay = 0f;
    public bool triggered = false;

    void Start()
    {
        // Сразу включаем эффект появления
        PlaySpawnEffect();
    }

    void PlaySpawnEffect()
    {
        if (spawnVFX != null)
            spawnVFX.SetActive(true);

        if (triggerVFX != null)
            triggerVFX.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            ActivateBomb();
        }
    }

    void ActivateBomb()
    {
        // выключаем первый эффект
        if (spawnVFX != null)
            spawnVFX.SetActive(false);

        // включаем второй эффект
        if (triggerVFX != null)
            triggerVFX.SetActive(true);

        // уничтожаем объект после проигрыша эффекта
        float delay = 1.5f; // под длительность VFX
        Destroy(gameObject, delay);
    }
}
