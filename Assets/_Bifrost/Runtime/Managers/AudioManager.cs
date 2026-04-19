using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _uIAudioSource;
    
    [SerializeField] private AudioSource _musicSource;

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
        _musicSource.volume = value;
    }

    public void SetUIVolume(float value)
    {
        
    }
    
    public float GetMusicVolume()
    {
        return _musicSource.volume;
    }

    public float GetUIVolume()
    {
        return _uIAudioSource.volume;
    }

    public void PlayUISound()
    {
        _uIAudioSource.PlayOneShot(_uIAudioSource.clip);
    }

    public void PlayMainMenuSound()
    {
        _musicSource.Play();
    }

    public void StopMainMenuSound()
    {
        _musicSource.Stop();
    }
}