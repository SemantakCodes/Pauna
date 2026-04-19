using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadingScreenUI;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public void LoadLevel(string sceneName)
    {
        // Start the background loading process
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        // 1. Show the loading UI
        loadingScreenUI.SetActive(true);

        // 2. Start loading the scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // 3. While the scene isn't finished loading...
        while (!operation.isDone)
        {
            // operation.progress goes from 0 to 0.9. We normalize it to 0-1.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // 4. Update UI
            progressBar.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";

            yield return null; // Wait for the next frame
        }
    }
}