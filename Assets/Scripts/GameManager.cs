using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Screens")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameplayUI;

    [Header("Cameras")]
    [SerializeField] private Camera menuCamera;
    [SerializeField] private Camera playerCamera;

    [Header("Player")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerHotbar playerHotbar;
    [SerializeField] private PlayerItemInteract playerItemInteract;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Enemies")]
    [SerializeField] private GameObject enemiesParent;

    private bool gameStarted;
    private bool gameOver;

    public bool GameStarted => gameStarted;
    public bool GameOver => gameOver;

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
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        gameStarted = false;
        gameOver = false;

        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (menuCamera != null)
            menuCamera.gameObject.SetActive(true);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);

        SetPlayerControls(false);
        SetEnemiesActive(false);

        Time.timeScale = 0f;
        UnlockCursor();
    }

    public void StartGame()
    {
        gameStarted = true;
        gameOver = false;

        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (gameplayUI != null)
            gameplayUI.SetActive(true);

        if (menuCamera != null)
            menuCamera.gameObject.SetActive(false);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        playerObject.transform.position = playerSpawnPoint.position;
        playerObject.transform.rotation = playerSpawnPoint.rotation;

        SetPlayerControls(true);
        SetEnemiesActive(true);

        Time.timeScale = 1f;
        LockCursor();
    }

    public void TriggerGameOver()
    {
        if (gameOver)
            return;

        gameOver = true;
        gameStarted = false;

        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        SetPlayerControls(false);
        SetEnemiesActive(false);

        Time.timeScale = 0f;
        UnlockCursor();

        Debug.Log("Game Over.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game.");

        Application.Quit();
    }

    private void SetPlayerControls(bool enabled)
    {
        if (playerController != null)
            playerController.enabled = enabled;

        if (playerHotbar != null)
            playerHotbar.enabled = enabled;

        if (playerItemInteract != null)
            playerItemInteract.enabled = enabled;
    }

    private void SetEnemiesActive(bool active)
    {
        if (enemiesParent != null)
            enemiesParent.SetActive(active);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}