using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _uIAudioSource;
    [SerializeField] private AudioSource _musicMainMenuSource;
    [SerializeField] private AudioSource _musicGameplaySource;
    [SerializeField] private AudioSource _musicCreditSource;
    [SerializeField] private AudioSource _openPortalAudioSource;
    [SerializeField] private AudioSource _closePortalAudioSource;
    [SerializeField] private AudioSource _putOnCrystalIntoPortalAudioSource;
    

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
        _musicMainMenuSource.volume = value;
        _musicCreditSource.volume = value;
        _musicGameplaySource.volume = value;
    }

    public void SetMusicSFXVolume(float value)
    {
        _uIAudioSource.volume = value;
        _openPortalAudioSource.volume = value;
        _closePortalAudioSource.volume = value;
        _putOnCrystalIntoPortalAudioSource.volume = value;
    }
    
    public float GetMusicVolume()
    {
        return _musicMainMenuSource.volume;
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
        _musicMainMenuSource.Play();
    }

    public void StopMainMenuSound()
    {
        _musicMainMenuSource.Stop();
    }

    public void PlayOpenPortalSound()
    {
        _openPortalAudioSource.PlayOneShot(_openPortalAudioSource.clip);
    }

    public void PlayClosePortalSound()
    {
        _closePortalAudioSource.PlayOneShot(_closePortalAudioSource.clip);
    }

    public void PlayPutOnCrystalIntoPortalSound()
    {
        _putOnCrystalIntoPortalAudioSource.PlayOneShot(_putOnCrystalIntoPortalAudioSource.clip);
    }
}