using System.Collections;
using UnityEngine;

public class ChangeMenuSlide : MonoBehaviour
{
    [SerializeField] private RectTransform firstObject;
    [SerializeField] private RectTransform secondObject;
    [SerializeField] private float slideUpDistance = 300f;
    [SerializeField] private float slideDuration = 0.35f;

    private bool hasSlid;

    public void OnClickChangeMenu()
    {
        if (hasSlid)
        {
            return;
        }

        hasSlid = true;
        StartCoroutine(SlideUpCoroutine());
    }

    private IEnumerator SlideUpCoroutine()
    {
        Vector2 firstStart = firstObject != null ? firstObject.anchoredPosition : Vector2.zero;
        Vector2 secondStart = secondObject != null ? secondObject.anchoredPosition : Vector2.zero;

        Vector2 firstTarget = firstStart + Vector2.up * slideUpDistance;
        Vector2 secondTarget = secondStart + Vector2.up * slideUpDistance;

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            t = t * t * (3f - 2f * t);

            if (firstObject != null)
            {
                firstObject.anchoredPosition = Vector2.Lerp(firstStart, firstTarget, t);
            }

            if (secondObject != null)
            {
                secondObject.anchoredPosition = Vector2.Lerp(secondStart, secondTarget, t);
            }

            yield return null;
        }

        if (firstObject != null)
        {
            firstObject.anchoredPosition = firstTarget;
        }

        if (secondObject != null)
        {
            secondObject.anchoredPosition = secondTarget;
        }
    }
}
