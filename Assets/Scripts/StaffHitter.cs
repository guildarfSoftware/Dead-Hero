using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffHitter : MonoBehaviour
{
    static float StaffDamage = 2;
    [SerializeField] LayerMask obstacleLayer;
    public static Action<Vector2> onObstacleHit;
    private void OnCollisionEnter2D(Collision2D other)
    {
        int otherLayerMask = 1 << other.gameObject.layer;
        if ((otherLayerMask & obstacleLayer) != 0)
        {
            ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
            other.GetContacts(contacts);
            Vector2 normalResult = Vector2.zero;
            foreach (var contact in contacts)
            {
                normalResult += contact.normal;
            }
            print(normalResult.normalized);
            onObstacleHit?.Invoke(normalResult.normalized);

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
