using _Bifrost.Runtime.Managers;
using UnityEngine;

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
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            case GameState.RUNNING:
                _playerController.EnableInput();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }
}