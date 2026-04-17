using _Bifrost.Runtime.Managers;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    
    [SerializeField] private PlayerController _playerController;
    
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _hudUI;
    
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
                _mainMenuUI.SetActive(false);
                break;
        }
    }
    public void ShowMainMenu()
    {
        _mainMenuUI.SetActive(true);
        _settingsUI.SetActive(false);
        _hudUI.SetActive(false);

        _playerController.DisableInput();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        _mainMenuUI.SetActive(false);
        _settingsUI.SetActive(false);
        _hudUI.SetActive(true);

        _playerController.EnableInput();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowSettings()
    {
        _mainMenuUI.SetActive(false);
        _settingsUI.SetActive(true);
        _hudUI.SetActive(false);

        _playerController.DisableInput();
    }
}