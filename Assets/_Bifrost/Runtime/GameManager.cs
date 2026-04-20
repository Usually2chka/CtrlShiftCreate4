using System;
using _Bifrost.Runtime.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using _Bifrost.UI.Controllers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState = GameState.NONE;
    
    [SerializeField] private PlayerController _playerController;
    [SerializeField] public CrystalDatabase _crystalDatabase;

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
        {
            if (_playerController != null && !_playerController.HUD.IsTutorialOpen && CurrentState != GameState.PAUSED)
                SetState(GameState.PAUSED);
            else
            {
                SetState(GameState.RUNNING);
                _playerController.HUD.Show();
                SettingsController.Hide();
            }
            
        }
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            EndGame();
    }

    private void Start()
    {
        isNoteOnceOpen = false;
        if (CurrentState == GameState.NONE) {
            SetState(GameState.PAUSED);
        }
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
                AudioManager.Instance.PlayMainMenuSound();
                break;

            case GameState.RUNNING:
                AudioManager.Instance.StopMainMenuSound();
                _playerController.EnableInput();
                _playerController.HideMainMenu();
                Time.timeScale = 1;
                if (!isNoteOnceOpen)
                    {
                        isNoteOnceOpen = true;
                        ShowNote();
                    }
                break;

            case GameState.CREDITS:
                AudioManager.Instance.PlayCreditsSound();
                _playerController.DisableInput();
                _playerController.HideMainMenu();
                break;
        }
    }

    public void EndGame()
    {
        isNoteOnceOpen = false;
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

    [SerializeField] private NoteController notePrefab;
    private bool isNoteOnceOpen = false;
    public void ShowNote()
    {
        var note = Instantiate(notePrefab);
        
        _playerController.DisableInputForUI();

        note.ShowNote();


        note.OnClosed += HandleNoteClosed;
    }
    private void HandleNoteClosed()
    {
        _playerController.EnableInputForUI();
    }
}