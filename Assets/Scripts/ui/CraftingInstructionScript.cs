using UnityEngine;
using System.Collections;


public class CraftingInstructionScript : MonoBehaviour
{

    public float fadeInDuration = 0.1f;
    public float fadeOutDuration = 0.1f;
    private CanvasGroup canvasGroup;
    private bool isVisible = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleVisibility();
        }
    }

    public void ToggleVisibility()
    {
        if (isVisible)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            StartCoroutine(FadeIn());
        }
        isVisible = !isVisible;
    }

    private IEnumerator FadeIn()
    {
        float duration = fadeInDuration;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        float duration = fadeOutDuration;
        float elapsedTime = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
            yield return null;
        }
    }
}
