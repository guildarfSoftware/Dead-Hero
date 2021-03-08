using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffHitter : MonoBehaviour
{
    static float StaffDamage = 2;
    [SerializeField] LayerMask obstacleLayer;
    public static Action onObstacleHit;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == obstacleLayer)
        {
            onObstacleHit?.Invoke();
        }

        IHitable hitable = other.gameObject.GetComponent<IHitable>();
        if (hitable != null)
        {
            hitable.Hit(StaffDamage);
            print("staff hit");
        }
    }

    static void SetStaffDamage(float amount)
    {
        StaffDamage = amount;
    }
}
