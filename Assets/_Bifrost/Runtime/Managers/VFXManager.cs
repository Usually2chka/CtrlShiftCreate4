using System;
using UnityEngine;
using UnityEngine.VFX;

namespace _Bifrost.Runtime.Managers
{
    public class VFXManager_1 : MonoBehaviour
    {
        public static VFXManager_1 s_instance { get; private set;}
        
        [SerializeField] private VisualEffect _vfxFog;
        [SerializeField] private VisualEffect _vfxFire;
        [SerializeField] private VisualEffect _vfxSnow;
        [SerializeField] private ParticleSystem _vfxLeaves;
        [SerializeField] private ParticleSystem _vfxSparks;
        public bool IsEnabled { get; private set; } = true;

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void VFXEnabled(bool value)
        {
            IsEnabled = value;
            // _vfxFog.enabled = value;
            // _vfxSnow.enabled = value;
            // _vfxFire.enabled = value;
            
            _vfxFog.gameObject.SetActive(value);
            _vfxFire.gameObject.SetActive(value);
            _vfxSnow.gameObject.SetActive(value);
            _vfxLeaves.gameObject.SetActive(value);
            _vfxSparks.gameObject.SetActive(value);
        }
    }
}