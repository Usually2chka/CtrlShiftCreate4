using System;
using _Bifrost.Runtime.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState = GameState.NONE;
    
    [SerializeField] private PlayerController _playerController;
    public PlayerController Player => _playerController;
    public string gameplaySceneName = "MyScene";
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            SetState(GameState.PAUSED);
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            EndGame();
    }

    private void Start()
    {
        if (CurrentState == GameState.NONE)
            SetState(GameState.PAUSED);
    }

    public void SetState(GameState newState)
    {
        Debug.Log($"Game state changed: {CurrentState} -> {newState}");
        CurrentState = newState;
        
        switch (newState)
        {
            case GameState.PAUSED:
                _playerController.DisableInput();
                _playerController.ShowMainMenu();
                Time.timeScale = 0;
                break;

            case GameState.RUNNING:
                _playerController.EnableInput();
                _playerController.HideMainMenu();
                Time.timeScale = 1;
                break;

            case GameState.CREDITS:
                _playerController.DisableInput();
                _playerController.HideMainMenu();
                break;
        }
    }

    public void EndGame()
    {
        StartCoroutine(EndGameRoutine());
    }

    [SerializeField] private CreditsController creditsPrefab;

    private IEnumerator EndGameRoutine()
    {
        SetState(GameState.CREDITS);

        yield return SceneManager.LoadSceneAsync(gameplaySceneName);

        var credits = Instantiate(creditsPrefab);
        credits.Show();

        yield return new WaitUntil(() => credits.IsFinished);

        Destroy(credits.gameObject);

        SetState(GameState.PAUSED);
    }

    public void RegisterPlayer(PlayerController player)
    {
        _playerController = player;
    }
}