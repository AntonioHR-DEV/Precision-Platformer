using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Image[] starImageArray;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button levelsButton;

    [Header("Stars")]
    [SerializeField] private Color starActivatedColor = Color.yellow;
    [SerializeField] private Color starDeactivatedColor = Color.grey;

    [Header("Slide Animation")]
    [SerializeField] private float hiddenY = -1200f;
    [SerializeField] private float shownY = 0f;
    [SerializeField] private float slideDuration = 0.5f;

    [Header("Title Blinking")]
    [SerializeField] private Color[] titleColors;
    [SerializeField] private float blinkInterval = .1f;
    private float elapsedBlinkTime = 0f;
    private int titleColorIndex = 0;

    public bool IsVisible { get; private set; }

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
        EndCheckpoint.OnEnded += EndCheckpoint_OnEnded;

        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        retryButton.onClick.AddListener(OnRetryClicked);
        levelsButton.onClick.AddListener(OnLevelsClicked);

        ResetStars();
        Hide();
    }

    private void Update()
    {
        elapsedBlinkTime += Time.unscaledDeltaTime;
        if (elapsedBlinkTime >= blinkInterval)
        {
            elapsedBlinkTime = 0f;
            titleColorIndex = (titleColorIndex + 1) % titleColors.Length;
            titleText.color = titleColors[titleColorIndex];
        }
    }

    private void OnDestroy()
    {
        EndCheckpoint.OnEnded -= EndCheckpoint_OnEnded;
    }

    // =========================================================================
    // Event Handlers
    // =========================================================================

    private void EndCheckpoint_OnEnded(object sender, EventArgs e)
    {
        UpdateVisual();

        float showDelay = 1f;
        Invoke(nameof(Show), showDelay);
        int levelIndex = SceneManager.GetActiveScene().buildIndex - 1;

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SetLevelCompleted(levelIndex, LevelTimer.Instance.GetStarRating());
        }
    }

    // =========================================================================
    // Visual
    // =========================================================================

    private void UpdateVisual()
    {
        statsText.text =
            "Time: " + LevelTimer.Instance.GetFormattedTime() + "\n" +
            "Deaths: " + PlayerController.Instance.DeathCount;

        ResetStars();

        int starCount = LevelTimer.Instance.GetStarRating();
        for (int i = 0; i < starCount; i++)
        {
            starImageArray[i].color = starActivatedColor;
        }
    }

    private void ResetStars()
    {
        foreach (Image starImage in starImageArray)
            starImage.color = starDeactivatedColor;
    }

    // =========================================================================
    // Show / Hide
    // =========================================================================

    private void Show()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(SlideRoutine(hiddenY, shownY));
        IsVisible = true;
    }

    private void Hide()
    {
        if (panelRect != null)
        {
            Vector2 pos = panelRect.anchoredPosition;
            pos.y = hiddenY;
            panelRect.anchoredPosition = pos;
        }
        gameObject.SetActive(false);
        IsVisible = false;
    }

    private IEnumerator SlideRoutine(float fromY, float toY)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);

            // Smoothstep for a natural ease-in-out feel
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

    // =========================================================================
    // Buttons
    // =========================================================================

    private void OnRetryClicked()
    {
        SceneLoader.Instance.ReloadCurrentScene();
    }

    private void OnNextLevelClicked()
    {
        SceneLoader.Instance.LoadNextScene();
    }

    private void OnLevelsClicked()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.LevelSelect);
    }
}