using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffHitter : MonoBehaviour
{
    static float StaffDamage = 2;
    public static Action onObstacleHit;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Map")
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
