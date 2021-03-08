using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomper : MonoBehaviour
{
    public float StompDamage = 1;
    public Action onStomp;
    PhysicsEntity physicsEntity;

    private void Start()
    {
        physicsEntity = transform.parent.GetComponent<PhysicsEntity>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (getRelativeVelocity(other).y >= 0) return;

        IStompable stompable = other.attachedRigidbody.GetComponent<IStompable>();
        if (stompable == null) return;

        stompable.Stomped(StompDamage);

        onStomp?.Invoke();
    }

    private Vector2 getRelativeVelocity(Collider2D other)
    {
        if (other.attachedRigidbody == null) return physicsEntity.Speed;
        
        PhysicsEntity otherEntity = other.attachedRigidbody.GetComponent<PhysicsEntity>();
        if (otherEntity == null) return physicsEntity.Speed;
        
        return physicsEntity.Speed - otherEntity.Speed ;
    }
}
