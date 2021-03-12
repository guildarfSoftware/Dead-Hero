using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitioner : MonoBehaviour
{
    [SerializeField] Vector2 targetSize;
    [SerializeField] RectTransform maskRectTransform;
    // Start is called before the first frame update
    void Awake()
    {
        maskRectTransform.sizeDelta = Vector2.zero;
    }

    public IEnumerator TransitionOut(float time)
    {
        if (time == 0)
        {
            maskRectTransform.sizeDelta = targetSize;
            yield break;
        }

        Vector2 transitionSpeed = targetSize/time;

        while (maskRectTransform.sizeDelta.magnitude < targetSize.magnitude)
        {
            maskRectTransform.sizeDelta += transitionSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator TransitionIn(float time)
    {
        if (time == 0)
        {
            maskRectTransform.sizeDelta = Vector2.zero;
            yield break;
        }

        Vector2 transitionSpeed = targetSize / time;
        while (maskRectTransform.sizeDelta.magnitude > 0)
        {
            Vector2 rest =transitionSpeed * Time.deltaTime;
            maskRectTransform.sizeDelta -= rest;

            if (maskRectTransform.sizeDelta.x < 0)
            {
                maskRectTransform.sizeDelta = Vector2.zero;
            }

            yield return null;
        }
    }

}
