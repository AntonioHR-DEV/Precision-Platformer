using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Requires:  SceneLoader singleton, SaveSystem singleton.
///            A Canvas with a panel RectTransform that slides in from off-screen.
///            GameInput must expose an OnPauseStarted event.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button levelsButton;
    [SerializeField] private GameObject settingsPanel;

    [Header("Slide Animation")]
    [SerializeField] private float hiddenY = -1200f;
    [SerializeField] private float shownY = 0f;
    [SerializeField] private float slideDuration = 0.3f;

    private bool isPaused;
    private Coroutine slideCoroutine;

    // =========================================================================
    // Unity Lifecycle
    // =========================================================================

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
        GameInput.Instance.OnPauseStarted += GameInput_OnPauseStarted;

        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        settingsButton.onClick.AddListener(OpenSettings);
        levelsButton.onClick.AddListener(GoToLevelSelect);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        SnapHidden();
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
            GameInput.Instance.OnPauseStarted -= GameInput_OnPauseStarted;
    }

    // =========================================================================
    // Input Callback
    // =========================================================================

    private void GameInput_OnPauseStarted(object sender, EventArgs e)
    {
        // Don't allow pausing if the level complete screen is showing
        if (LevelCompleteUI.Instance != null && LevelCompleteUI.Instance.IsVisible) return;

        TogglePause();
    }

    // =========================================================================
    // Pause / Resume
    // =========================================================================

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SlideIn();
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SlideOut();
    }

    // =========================================================================
    // Button Handlers
    // =========================================================================

    private void Restart()
    {
        SceneLoader.Instance.ReloadCurrentScene();
    }

    private void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void GoToLevelSelect()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.LevelSelect);
    }

    // =========================================================================
    // Slide Animation
    // =========================================================================

    private void SlideIn()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        gameObject.SetActive(true);
        slideCoroutine = StartCoroutine(SlideRoutine(hiddenY, shownY));
    }

    private void SlideOut()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideOutRoutine());
    }

    private void SnapHidden()
    {
        if (panelRect == null) return;
        Vector2 pos = panelRect.anchoredPosition;
        pos.y = hiddenY;
        panelRect.anchoredPosition = pos;
        gameObject.SetActive(false);
    }

    private IEnumerator SlideRoutine(float fromY, float toY)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float smoothT = t * t * (3f - 2f * t);

            Vector2 pos = panelRect.anchoredPosition;
            pos.y = Mathf.Lerp(fromY, toY, smoothT);
            panelRect.anchoredPosition = pos;

            yield return null;
        }

        Vector2 finalPos = panelRect.anchoredPosition;
        finalPos.y = toY;
        panelRect.anchoredPosition = finalPos;
    }

    private IEnumerator SlideOutRoutine()
    {
        yield return StartCoroutine(SlideRoutine(shownY, hiddenY));
        gameObject.SetActive(false);
    }

    // =========================================================================
    // Public State
    // =========================================================================

    public bool IsPaused => isPaused;
}