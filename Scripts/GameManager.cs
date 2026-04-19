using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Needed if  want to display text on screen

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Creates a Singleton so other scripts can find it easily

    [Header("Win Settings")]
    public int keysCollected = 0;
    public int totalKeysToWin = 8;
    public string winSceneName = "WinMenu"; // Change this to your actual win scene name

    [Header("UI (Optional)")]
    public TextMeshProUGUI keyUIText; 

    private void Awake()
    {
        // Setup the Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddKey()
    {
        keysCollected++;
        UpdateUI();

        // Optional: Make a loud scary noise when they pick up a key!
        
        if (keysCollected >= totalKeysToWin)
        {
            WinGame();
        }
    }

    private void UpdateUI()
    {
        if (keyUIText != null)
        {
            keyUIText.text = "Keys: " + keysCollected + " / " + totalKeysToWin;
        }
    }

    private void WinGame()
    {
        // Unlock the mouse cursor so they can click buttons on the Win Screen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SceneManager.LoadScene(winSceneName);
    }
}