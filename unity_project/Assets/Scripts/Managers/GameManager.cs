using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Systems")]
    public PlayerController player;
    public MissionManager missionManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public AITeamCoordinator teamCoordinator;
    
    [Header("Game State")]
    public GameState currentGameState;
    public int playerTokens;
    public int playerLevel;
    public string currentScene;
    
    [Header("Save System")]
    public bool autoSave = true;
    public float autoSaveInterval = 300f; // 5 minutes
    private float lastSaveTime;
    
    [Header("Game Settings")]
    public float mouseSensitivity = 2f;
    public bool invertYAxis = false;
    public float masterVolume = 1f;
    public Difficulty gameDifficulty = Difficulty.Normal;
    
    // Events
    public System.Action<GameState> OnGameStateChanged;
    public System.Action<int> OnTokensChanged;
    public System.Action<int> OnPlayerLevelChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeGameSystems();
    }
    
    void Start()
    {
        ChangeGameState(GameState.MainMenu);
        lastSaveTime = Time.time;
    }
    
    void Update()
    {
        // Auto-save system
        if (autoSave && Time.time - lastSaveTime >= autoSaveInterval)
        {
            SaveGame();
            lastSaveTime = Time.time;
        }
        
        // Handle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    
    void InitializeGameSystems()
    {
        // Initialize all game systems
        missionManager = FindObjectOfType<MissionManager>();
        uiManager = FindObjectOfType<UIManager>();
        audioManager = FindObjectOfType<AudioManager>();
        teamCoordinator = FindObjectOfType<AITeamCoordinator>();
        player = FindObjectOfType<PlayerController>();
        
        // Load player preferences
        LoadSettings();
        
        Debug.Log("Game systems initialized successfully");
    }
    
    public void ChangeGameState(GameState newState)
    {
        GameState previousState = currentGameState;
        currentGameState = newState;
        
        OnGameStateChanged?.Invoke(newState);
        
        Debug.Log($"Game state changed from {previousState} to {newState}");
        
        // State-specific logic
        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                break;
                
            case GameState.InMission:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                break;
                
            case GameState.MissionComplete:
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.None;
                break;
                
            case GameState.GameOver:
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }
    
    public void AddTokens(int amount)
    {
        playerTokens += amount;
        OnTokensChanged?.Invoke(playerTokens);
        
        Debug.Log($"Tokens updated: {playerTokens} (+{amount})");
    }
    
    public void SpendTokens(int amount)
    {
        if (playerTokens >= amount)
        {
            playerTokens -= amount;
            OnTokensChanged?.Invoke(playerTokens);
            Debug.Log($"Tokens spent: {amount}. Remaining: {playerTokens}");
        }
        else
        {
            Debug.LogWarning("Not enough tokens to spend!");
        }
    }
    
    public void SetPlayerLevel(int level)
    {
        playerLevel = level;
        OnPlayerLevelChanged?.Invoke(playerLevel);
    }
    
    void TogglePauseMenu()
    {
        if (currentGameState == GameState.InMission)
        {
            ChangeGameState(GameState.Paused);
            uiManager.ShowPauseMenu();
        }
        else if (currentGameState == GameState.Paused)
        {
            ChangeGameState(GameState.InMission);
            uiManager.HidePauseMenu();
        }
    }
    
    public void SaveGame()
    {
        // Save game data
        PlayerPrefs.SetInt("PlayerTokens", playerTokens);
        PlayerPrefs.SetInt("PlayerLevel", playerLevel);
        PlayerPrefs.SetString("CurrentScene", currentScene);
        PlayerPrefs.Save();
        
        Debug.Log("Game saved successfully");
    }
    
    public void LoadGame()
    {
        // Load game data
        playerTokens = PlayerPrefs.GetInt("PlayerTokens", 0);
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        currentScene = PlayerPrefs.GetString("CurrentScene", "MainMenu");
        
        Debug.Log("Game loaded successfully");
    }
    
    void LoadSettings()
    {
        // Load player settings
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        invertYAxis = PlayerPrefs.GetInt("InvertYAxis", 0) == 1;
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        
        string difficultyString = PlayerPrefs.GetString("GameDifficulty", "Normal");
        gameDifficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), difficultyString);
    }
    
    public void SaveSettings()
    {
        // Save player settings
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("InvertYAxis", invertYAxis ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetString("GameDifficulty", gameDifficulty.ToString());
        PlayerPrefs.Save();
    }
    
    public void QuitToMainMenu()
    {
        SaveGame();
        // Load main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        ChangeGameState(GameState.MainMenu);
    }
    
    public void QuitGame()
    {
        SaveGame();
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}

public enum GameState
{
    MainMenu,
    InMission,
    Paused,
    MissionComplete,
    GameOver,
    Loading
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Expert
}
