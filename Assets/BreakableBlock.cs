using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour, IHitable
{
    [SerializeField] Animation _animation;
    private float animationTime = 0.5f;

    public void Hit(float damage)
    {
        StartCoroutine(DestroyBlock());
    }

    IEnumerator DestroyBlock()
    {

        _animation.Play();
        float animationCounter = animationTime;
        while (animationCounter > 0)
        {
            yield return new WaitForSeconds(0.2f);
            animationCounter -= 0.2f;
        }
        Destroy(gameObject);
    }
}
