using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*
 * Author(s): Anthony L
 * Date: 6.2.26
 * Notes:
 *  - Handles scene loading with:
 *      - Minimum loading screen duration
 *      - Loading progress bar
 *      - Fade in transition
 *  - I'd like to add fade out however that would require some reworking
 *    to make the Loading Canvas and MenuManager DoNotDestroyOnLoad objects.
 *    Since the class works fine I'm going to come back to this rework at a 
 *    more appropriate time - and start working on higher priority tasks. 
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

    /// <summary>
    /// Loads a scene using its build index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    /// <summary>
    /// Loads the main game scene
    /// </summary>
    public void LoadGame()
    {
        StartCoroutine(LoadSceneRoutine(1));
    }

    /// <summary>
    /// Handles asynchronous scene loading
    /// </summary>
    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {

        if (loadingScreen == null || loadingSlider == null || loadingCanvasGroup == null)
        {
            Debug.LogError("Loading screen, slider, or canvas group is missing");
            yield break;
        }

        loadingScreen.SetActive(true);

        // reset values
        loadingCanvasGroup.alpha = 0f;
        loadingSlider.value = 0f;

        // fade in
        yield return StartCoroutine(FadeInLoadingScreen());

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // prevent immediate scene activation
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        // wait until loading is complete AND minimum time has passed
        while (operation.progress < 0.9f || elapsedTime < minimumLoadingTime)
        {
            elapsedTime += Time.deltaTime;

            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);

            // smooth progress bar
            loadingSlider.value = Mathf.Min(loadProgress, timeProgress);

            yield return null;
        }

        // fully loaded
        loadingSlider.value = 1f;

        // small pause so the full bar is visible
        yield return new WaitForSeconds(0.2f);

        // activate scene
        operation.allowSceneActivation = true;
    }

    /// <summary>
    /// Fades the loading screen in
    /// </summary>
    private IEnumerator FadeInLoadingScreen()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            loadingCanvasGroup.alpha =
                Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            yield return null;
        }

        loadingCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Closes the application. Only works in a desktop build.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}