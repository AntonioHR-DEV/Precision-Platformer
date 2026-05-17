using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton that handles all scene loading with a fade to black transition.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public enum Scene
    {
        MainMenu,
        LevelSelect,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8,
        Level9,
        Level10
    }

    public static SceneLoader Instance { get; private set; }


    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.4f;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(FadeIn());
    }

    public void LoadScene(Scene scene)
    {
        StartCoroutine(LoadSceneRoutine(scene));
    }

    public void LoadSceneByIndex(int buildIndex)
    {
        StartCoroutine(LoadSceneByIndexRoutine(buildIndex));
    }

    public void ReloadCurrentScene()
    {
        LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Load the next scene in Build Settings order.
    /// If the current scene is the last one, loads the Level Select screen.
    /// </summary>
    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            StartCoroutine(LoadSceneByIndexRoutine(nextIndex));
        else
            LoadScene(Scene.LevelSelect);
    }

    // =========================================================================
    // Fade Routines
    // =========================================================================

    private IEnumerator LoadSceneRoutine(Scene scene)
    {
        yield return StartCoroutine(FadeOut());

        AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString());
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneByIndexRoutine(int buildIndex)
    {
        yield return StartCoroutine(FadeOut());

        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        // Block interactions as soon as the fade starts
        fadeImage.raycastTarget = true;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        // Unblock interactions once the screen is fully clear!
        fadeImage.raycastTarget = false;
    }
}