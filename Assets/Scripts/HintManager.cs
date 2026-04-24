using System.Collections;
using TMPro;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [Header("Hint UI")]
    public TextMeshProUGUI hintText;

    [Header("Fade Settings")]
    public float fadeDuration = 5f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (hintText != null)
        {
            hintText.text = string.Empty;
            SetAlpha(0f);
        }
    }

    public void ShowHint(string text)
    {
        if (hintText == null) return;

        hintText.text = text;
        SetAlpha(1f);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutHint());
    }

    private IEnumerator FadeOutHint()
    {
        yield return new WaitForSeconds(fadeDuration);

        float elapsed = 0f;
        float startAlpha = hintText.color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        hintText.text = string.Empty;
    }

    private void SetAlpha(float alpha)
    {
        Color color = hintText.color;
        color.a = alpha;
        hintText.color = color;
    }
}