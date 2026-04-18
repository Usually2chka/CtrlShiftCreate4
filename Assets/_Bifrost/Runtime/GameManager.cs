using System;
using _Bifrost.Runtime.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    
    [SerializeField] private PlayerController _playerController;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            SetState(GameState.PAUSED);
    }

    private void Start()
    {
        SetState(GameState.PAUSED);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        
        switch (newState)
        {
            case GameState.PAUSED:
                _playerController.DisableInput();
                _playerController.ShowMainMenu();
                break;

            case GameState.RUNNING:
                _playerController.EnableInput();
                _playerController.HideMainMenu();
                break;
        }
    }
}