using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Settings")]
    public string gameSceneName; 
    public string infoSceneName;
    public string mainMenuSceneName;
    public LoadingScreen loadingScreen; 
    public string controlScene;
    public GameObject controlObj;

    public void PlayGame()
    {
        // Loads the scene by the name provided in the inspector
        loadingScreen.LoadLevel(gameSceneName);
    }

    public void Info()
    {
        loadingScreen.LoadLevel(infoSceneName);
    }

    public void ReturnToMainMenu()
    {
        loadingScreen.LoadLevel(mainMenuSceneName);
    }
    
    public void ShowControl()
    {
        controlObj.SetActive(true);
    }
    public void HideControl()
    {
        controlObj.SetActive(false);
    }

    public void QuitGame()
    {
        
        Application.Quit();
    }
}