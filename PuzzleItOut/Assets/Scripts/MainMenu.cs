using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * Author(s): Anthony L
 * Date: 6.1.26
 * Notes:
 *  - This class has a minimum loading time variable so the loading screen doesnt instantly appear then disappear
 */
public class MainMenu : MonoBehaviour
{
    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public CanvasGroup loadingCanvasGroup;

    [Header("Loading Settings")]
    public float minimumLoadingTime = 2f;
    public float fadeDuration = 0.5f;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    public void LoadGame()
    {
        StartCoroutine(LoadSceneRoutine(1));
    }

    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        if (loadingScreen == null || loadingSlider == null || loadingCanvasGroup == null)
        {
            Debug.LogError("Loading screen, slider, or canvas group is missing");
            yield break;
        }

        // Enable loading screen
        loadingScreen.SetActive(true);

        // Start transparent
        loadingCanvasGroup.alpha = 0f;
        loadingSlider.value = 0f;

        // Fade in
        yield return StartCoroutine(FadeInLoadingScreen());

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // Prevent scene from activating immediately
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        // Wait until BOTH loading is done AND minimum time has passed
        while (operation.progress < 0.9f || elapsedTime < minimumLoadingTime)
        {
            elapsedTime += Time.deltaTime;

            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);

            loadingSlider.value = Mathf.Min(loadProgress, timeProgress);

            yield return null;
        }

        loadingSlider.value = 1f;

        // Activate scene
        operation.allowSceneActivation = true;
    }

    private IEnumerator FadeInLoadingScreen()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            loadingCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            yield return null;
        }

        loadingCanvasGroup.alpha = 1f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}