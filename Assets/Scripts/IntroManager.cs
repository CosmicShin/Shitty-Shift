using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public static bool IsIntroActive { get; private set; }

    [Header("Intro UI")]
    public GameObject introPanel;
    public Image backgroundFadeImage;
    public TextMeshProUGUI narrationText;

    [Header("Narration")]
    public string[] narrationLines = new string[0];
    public float typewriterDelay = 0.04f;
    public float lineHoldDuration = 1.2f;
    public float textFadeDuration = 0.6f;

    [Header("Background Fade")]
    public float backgroundFadeDuration = 1f;

    private Coroutine introCoroutine;
    private bool skipRequested;

    private void Awake()
    {
        IsIntroActive = true;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (backgroundFadeImage != null)
        {
            Color color = backgroundFadeImage.color;
            color.a = 1f;
            backgroundFadeImage.color = color;
        }

        if (narrationText != null)
        {
            narrationText.text = string.Empty;
            Color color = narrationText.color;
            color.a = 1f;
            narrationText.color = color;
        }

        Time.timeScale = 0f;
    }

    private void Start()
    {
        introCoroutine = StartCoroutine(RunIntro());
    }

    private void Update()
    {
        if (!IsIntroActive)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SkipIntro();
    }

    public void SkipIntro()
    {
        if (!IsIntroActive)
            return;

        skipRequested = true;
        if (introCoroutine != null)
            StopCoroutine(introCoroutine);

        StartCoroutine(CompleteIntro());
    }

    private IEnumerator RunIntro()
    {
        yield return FadeImageAlpha(backgroundFadeImage, 1f, 0f, backgroundFadeDuration);

        if (skipRequested)
        {
            yield return CompleteIntro();
            yield break;
        }

        foreach (string line in narrationLines)
        {
            if (skipRequested)
                break;

            yield return ShowLine(line);

            if (skipRequested)
                break;

            yield return new WaitForSecondsRealtime(lineHoldDuration);
            yield return FadeTextAlpha(1f, 0f, textFadeDuration);
        }

        if (!skipRequested)
            yield return CompleteIntro();
    }

    private IEnumerator ShowLine(string line)
    {
        if (narrationText == null)
            yield break;

        narrationText.text = string.Empty;

        Color color = narrationText.color;
        color.a = 1f;
        narrationText.color = color;

        foreach (char c in line)
        {
            if (skipRequested)
                yield break;

            narrationText.text += c;
            yield return new WaitForSecondsRealtime(typewriterDelay);
        }
    }

    private IEnumerator FadeTextAlpha(float from, float to, float duration)
    {
        if (narrationText == null)
            yield break;

        float timer = 0f;
        Color color = narrationText.color;

        while (timer < duration && !skipRequested)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(from, to, timer / Mathf.Max(duration, 0.0001f));
            narrationText.color = color;
            yield return null;
        }

        color.a = to;
        narrationText.color = color;
    }

    private IEnumerator FadeImageAlpha(Image image, float from, float to, float duration)
    {
        if (image == null)
            yield break;

        float timer = 0f;
        Color color = image.color;
        color.a = from;
        image.color = color;

        while (timer < duration && !skipRequested)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(from, to, timer / Mathf.Max(duration, 0.0001f));
            image.color = color;
            yield return null;
        }

        color.a = to;
        image.color = color;
    }

    private IEnumerator CompleteIntro()
    {
        if (backgroundFadeImage != null)
        {
            Color color = backgroundFadeImage.color;
            color.a = 0f;
            backgroundFadeImage.color = color;
        }

        if (narrationText != null)
            narrationText.text = string.Empty;

        if (introPanel != null)
            introPanel.SetActive(false);

        IsIntroActive = false;
        skipRequested = false;
        Time.timeScale = 1f;

        yield return null;
    }
}
