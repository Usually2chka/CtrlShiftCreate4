using System;
using UnityEngine;
using UnityEngine.VFX;

namespace _Bifrost.Runtime.Managers
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager s_instance { get; private set;}
        
        [SerializeField] private VisualEffect _vfxFog;
        [SerializeField] private VisualEffect _vfxFire;
        [SerializeField] private VisualEffect _vfxSnow;
        [SerializeField] private ParticleSystem _vfxLeaves;
        [SerializeField] private ParticleSystem _vfxSparks;

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