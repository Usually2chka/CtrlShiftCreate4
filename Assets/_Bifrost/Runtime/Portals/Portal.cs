using UnityEngine;
using System.Linq;
using System;

namespace _Bifrost.Runtime.Portals
{
    public class Portal : MonoBehaviour
{
    public PortalConfig config;
    public PortalState state;

    private IWorldEffect worldEffect;
    [SerializeField] private int _stabilityLevel = 0; // текущий уровень стабильности
    [SerializeField] private Renderer portalTextRenderer; // рендерер для изменения материала портала
    [SerializeField] private GameObject[] portalVisual; // визуальная часть портала
    [Serializable] public class InteractDoorMaterials
    {
        public Renderer renderer;
        public Material openMaterial;
        public Material closedMaterial;
    }
    [SerializeField] private InteractDoorMaterials interactDoorMaterials;

    public int StabilityLevel => _stabilityLevel;
    
    // событие изменения состояния портала
    public event Action<PortalState> OnStateChanged;

    private void Awake()
    {
        worldEffect = GetComponent<IWorldEffect>();
        if (worldEffect == null)
        {
            Debug.LogError("IWorldEffect component is missing!", this);
        }
        if (portalTextRenderer == null)
        {
            Debug.LogWarning("Portal text renderer is not assigned!", this);
        } else
        {
            // Инициализируем материал портала на основе начального состояния
            portalTextRenderer.material.SetFloat("_State", _stabilityLevel);
        }
        portalVisual?.ToList().ForEach(v => v.SetActive(false)); // при старте все порталы неактивны
        if (interactDoorMaterials.renderer != null)
        {
            // Изначально устанавливаем материал в закрытое состояние
            interactDoorMaterials.renderer.material = interactDoorMaterials.closedMaterial;
        }
    }

    public void Open()
    {
        if (state != PortalState.Closed) return;
        if (WorldEffectManager.Instance == null)
        {
            Debug.LogError("WorldEffectManager Instance is NULL!");
            return;
        }

        AudioManager.Instance.PlayOpenPortalSound(); // воспроизводим звук открытия
        portalVisual?.ToList().ForEach(v => v.SetActive(true)); // активируем визуальную часть портала
        if (interactDoorMaterials.renderer != null)
        {
            // Устанавливаем материал в открытое состояние
            interactDoorMaterials.renderer.material = interactDoorMaterials.openMaterial;
        }

        // активируем кристаллы этого типа портала
        ActivateCrystals(config.worldType);

        RecalculateStability(); // при открытии портала пересчитываем стабильность на основе текущих кристаллов в сокетах
        state = PortalState.OpenUnstable;
        WorldEffectManager.Instance.RegisterEffect(worldEffect);
        ActivatePortal(true);
        
        // пересчитываем стабильность всех порталов
        RecalculateAllPortalsStability();
        
        // уведомляем об изменении состояния
        OnStateChanged?.Invoke(state);
        TextureSwitcher.Instance.HideLayer(config.worldType); // Скрыть слой мира
        RecalculateStability(); // обновляем материал портала на основе текущей стабильности
        CheckStability();
    }

    public void Close()
    {
        AudioManager.Instance.PlayClosePortalSound(); // воспроизводим звук закрытия
        if (interactDoorMaterials.renderer != null)
        {
            // Устанавливаем материал в закрытое состояние
            interactDoorMaterials.renderer.material = interactDoorMaterials.closedMaterial;
        }

        // деактивируем кристаллы этого типа портала
        DeactivateCrystals(config.worldType);

        state = PortalState.Closed;
        WorldEffectManager.Instance.RemoveEffect(worldEffect);
        ActivatePortal(false);
        portalVisual?.ToList().ForEach(v => v.SetActive(false)); // деактивируем визуальную часть портала

        // пересчитываем стабильность всех порталов
        RecalculateAllPortalsStability();
        
        // уведомляем об изменении состояния
        OnStateChanged?.Invoke(state);
        TextureSwitcher.Instance.HideLayer(config.worldType); // Скрыть слой мира
    }

    public void Stabilize()
    {
        if (state != PortalState.OpenUnstable) return;

        state = PortalState.Stabilized;
        WorldEffectManager.Instance.RemoveEffect(worldEffect);
        OnStateChanged?.Invoke(state);
        TextureSwitcher.Instance.ShowLayer(config.worldType); // Показать слой мира
    }

    public void Destabilize()
    {
        if (state != PortalState.Stabilized) return;

        state = PortalState.OpenUnstable;
        WorldEffectManager.Instance.RegisterEffect(worldEffect);
        OnStateChanged?.Invoke(state);
        TextureSwitcher.Instance.HideLayer(config.worldType); // Скрыть слой мира
    }

    public void AddCrystal(Crystal crystal)
    {
        // если кристалл неактивный, не добавляем его к стабильности
        if (!crystal.IsActive) return;
        
        if (state == PortalState.Closed) return;

        // находим требование для этого типа кристалла
        var requirement = config.stabilizationCost.Find(r => r.type == crystal.CrystalType);
        if (requirement.cost > 0)
        {
            _stabilityLevel += requirement.cost;
            CheckStability();
        }
    }

    public void RemoveCrystal(Crystal crystal)
    {
        // если кристалл неактивный, он уже не был добавлен к стабильности
        if (!crystal.IsActive) return;
        
        if (state == PortalState.Closed) return;

        // находим требование для этого типа кристалла
        var requirement = config.stabilizationCost.Find(r => r.type == crystal.CrystalType);
        if (requirement.cost > 0)
        {
            _stabilityLevel -= requirement.cost;
            if (_stabilityLevel < 0) _stabilityLevel = 0;
            CheckStability();
        }
    }

    public void ResetStability()
    {
        _stabilityLevel = 0;
        if (state == PortalState.Stabilized)
        {
            Destabilize();
        }
    }

    public void RecalculateStability()
    {
        // сбросить стабильность
        ResetStability();
        
        // найти все берущие сокеты этого портала
        var receivingSockets = FindObjectsByType<_Bifrost.Runtime.Managers.GamePlay.SocketPortal>(FindObjectsSortMode.None)
            .Where(s => s.SocketType == _Bifrost.Runtime.Managers.GamePlay.SocketType.Receiving && s.LinkedPortal == this);
            
        // заново добавить все кристаллы из этих сокетов
        foreach (var socket in receivingSockets)
        {
            if (socket.HasItem())
            {
                var crystal = socket.GetCurrentCrystal();
                if (crystal != null)
                {
                    AddCrystal(crystal);
                }
            }
        }
    }

    private void CheckStability()
    {
        if (_stabilityLevel == 3 && state == PortalState.OpenUnstable)
        {
            Stabilize();
        }
        else if (_stabilityLevel != 3 && state == PortalState.Stabilized)
        {
            Destabilize();
        }
        if (portalTextRenderer != null)
            portalTextRenderer?.material.SetFloat("_State", _stabilityLevel);
    }

    private void DeactivateCrystals(WorldType type)
    {
        // найти все кристаллы этого типа и деактивировать их
        var crystals = FindObjectsByType<Crystal>(FindObjectsSortMode.None)
            .Where(c => c.CrystalType == type);
        
        foreach (var crystal in crystals)
        {
            crystal.SetActive(false);
        }
        
        // обновить спрайты в HUD
        UpdateHUDCrystals();
    }

    private void ActivateCrystals(WorldType type)
    {
        // найти все кристаллы этого типа и активировать их
        var crystals = FindObjectsByType<Crystal>(FindObjectsSortMode.None)
            .Where(c => c.CrystalType == type);
        
        foreach (var crystal in crystals)
        {
            crystal.SetActive(true);
        }
        
        // обновить спрайты в HUD
        UpdateHUDCrystals();
    }

    private void UpdateHUDCrystals()
    {
        // найти HUDController и обновить спрайты кристаллов
        var hudController = FindFirstObjectByType<_Bifrost.UI.Controllers.HUDController>();
        if (hudController != null)
        {
            hudController.RefreshAllSlots();
        }
    }

    private static void RecalculateAllPortalsStability()
    {
        // пересчитать стабильность всех открытых порталов
        var portals = FindObjectsByType<Portal>(FindObjectsSortMode.None);
        foreach (var portal in portals)
        {
            if (portal.state != PortalState.Closed)
            {
                portal.RecalculateStability();
            }
        }
    }

    private void ActivatePortal(bool active)
    {
        // визуал / интеракция
    }
}
}