using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource uiAudioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // управление музыкой
    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }
}