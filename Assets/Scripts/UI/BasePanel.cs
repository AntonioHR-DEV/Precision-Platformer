using System;
using System.Collections;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    [Header("Slide Animation")]
    [SerializeField] protected RectTransform panelRect;
    [SerializeField] protected float hiddenY = -1200f;
    [SerializeField] protected float shownY = 0f;
    [SerializeField] protected float slideDuration = 0.3f;

    private Coroutine slideCoroutine;

    private Action onCloseCallback;

    public bool IsVisible { get; protected set; }

    public virtual void Show(Action onClose = null)
    {
        onCloseCallback = onClose;
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        slideCoroutine = StartCoroutine(SlideRoutine(hiddenY, shownY));
        IsVisible = true;
    }

    public virtual void ShowWithDelay(float delay, Action onClose = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowWithDelayRoutine(delay, onClose));
    }

    private IEnumerator ShowWithDelayRoutine(float delay, Action onClose)
    {
        yield return new WaitForSecondsRealtime(delay);
        Show(onClose);
    }

    public virtual void Hide()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideOutRoutine());
        IsVisible = false;
    }

    public virtual void SnapHidden()
    {
        if (panelRect == null) return;
        Vector2 pos = panelRect.anchoredPosition;
        pos.y = hiddenY;
        panelRect.anchoredPosition = pos;
        gameObject.SetActive(false);
        IsVisible = false;
        onCloseCallback?.Invoke();
        onCloseCallback = null;
    }

    protected IEnumerator SlideRoutine(float fromY, float toY)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            float smoothT = t * t * (3f - 2f * t);

            if (panelRect != null)
            {
                Vector2 pos = panelRect.anchoredPosition;
                pos.y = Mathf.Lerp(fromY, toY, smoothT);
                panelRect.anchoredPosition = pos;
            }

            yield return null;
        }

        if (panelRect != null)
        {
            Vector2 finalPos = panelRect.anchoredPosition;
            finalPos.y = toY;
            panelRect.anchoredPosition = finalPos;
        }
    }

    private IEnumerator SlideOutRoutine()
    {
        yield return StartCoroutine(SlideRoutine(shownY, hiddenY));
        gameObject.SetActive(false);

        onCloseCallback?.Invoke();
        onCloseCallback = null;
    }
}
