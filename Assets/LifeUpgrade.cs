using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUpgrade : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Health health = other.gameObject.GetComponent<Health>();
        if(health != null)
        {
            health.Heal(1);
            Destroy(gameObject);
        }
    }
}
