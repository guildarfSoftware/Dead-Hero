using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PhysicsEntity : MonoBehaviour
{
    [SerializeField] protected float gravityMultiplier = 2.5f;
    protected float dragCoeficient = 0;
    protected Collider2D _mainCollider;
    [SerializeField] protected LayerMask SolidLayer;
    public enum Facing
    {
        right,
        left,
    }

    

    float minSpeedForDynamic = 0.01f;

    bool movingHorizontally { get => Mathf.Abs(Speed.x) > minSpeedForDynamic; }
    bool movingVertically { get => Mathf.Abs(Speed.y) > minSpeedForDynamic; }

    float dinamicSkinWidth = 0.3f;
    float dinamicSkinHeight = 0.2f;
    float staticSkinSize = 0.015f;
    RaycastHit2D[] hits = new RaycastHit2D[16];
    public const Facing FacingRight = Facing.right;
    public const Facing FacingLeft = Facing.left;
    protected Facing facingDirection;
    public Vector2 Speed;
    protected bool IgnoreGravity, IgnoreDrag;
    Bounds collisionBounds;
    Vector2 currentSkinSize;
    void Move(Vector2 movement)
    {
        if (_mainCollider == null) _mainCollider = GetComponent<Collider2D>();

        UpdateSkinwidth();
        ExtDebug.DrawBox(collisionBounds.center, collisionBounds.extents, Quaternion.identity, Color.green);

        CheckHorizontalMovement(ref movement);
        CheckVerticalMovement(ref movement);

        transform.Translate(movement);
        ExtDebug.DrawBox(collisionBounds.center + (Vector3)movement, collisionBounds.extents, Quaternion.identity, Color.red);
    }

    private void UpdateSkinwidth()
    {
        collisionBounds = _mainCollider.bounds;

        collisionBounds.Expand(-staticSkinSize);

        Vector2 skinAdaptedSize = collisionBounds.size;

        if (movingHorizontally)
        {
            skinAdaptedSize.y = Mathf.Max(skinAdaptedSize.y - dinamicSkinHeight * 2, 0.01f);
        }
        if (movingVertically)
        {
            skinAdaptedSize.x = Mathf.Max(skinAdaptedSize.x - dinamicSkinWidth * 2, 0.01f);
        }
        collisionBounds.size = skinAdaptedSize;

        currentSkinSize = (_mainCollider.bounds.size - collisionBounds.size)/2;
    }

    private void CheckHorizontalMovement(ref Vector2 movement)
    {
        float sign = Math.Sign(movement.x);
        float distance = Mathf.Abs(movement.x) + currentSkinSize.x;
        Vector2 direction = Vector2.right * sign;
        int nHits = Physics2D.BoxCastNonAlloc(collisionBounds.center, collisionBounds.size, 0, direction, hits, distance, SolidLayer);
        float closestHitDistance = float.MaxValue;
        ExtDebug.DrawBox(collisionBounds.center + (Vector3)direction * distance, collisionBounds.extents, Quaternion.identity, Color.magenta);
        if (nHits == 0) return;

        for (int i = 0; i < nHits; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.distance < closestHitDistance)
            {
                closestHitDistance = hit.distance;
            }
        }
        movement.x = (closestHitDistance - currentSkinSize.x) * sign;
        Speed.x = 0;
    }

    private void CheckVerticalMovement(ref Vector2 movement)
    {
        float sign = Math.Sign(movement.y);
        float distance = Mathf.Abs(movement.y) + currentSkinSize.y;
        Vector2 direction = Vector2.up * sign;
        int nHits = Physics2D.BoxCastNonAlloc(collisionBounds.center, collisionBounds.size, 0, direction, hits, distance, SolidLayer);
        float closestHitDistance = float.MaxValue;

        ExtDebug.DrawBox(collisionBounds.center + (Vector3)direction * distance, collisionBounds.extents, Quaternion.identity, Color.yellow);
        if (nHits == 0) return;

        for (int i = 0; i < nHits; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.distance < closestHitDistance)
            {
                closestHitDistance = hit.distance;
            }
        }
        if(Speed.y>0)
        {
            print("vertical hit");
        }
        movement.y = (closestHitDistance - currentSkinSize.y) * sign;
        Speed.y = 0;
    }

    protected void Update()
    {
        Move(Speed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //calculateGravity
        if (!IgnoreGravity)
        {
            Speed.y += gravityMultiplier * Physics2D.gravity.y * Time.deltaTime;
        }
        //calculate drag
        if (!IgnoreDrag)
        {
            float sign = Mathf.Sign(Speed.x);
            float absSpeed = Mathf.Abs(Speed.x);

            absSpeed -= absSpeed * dragCoeficient;
            float newVelocity = sign * Mathf.Max(absSpeed, 0);

            Speed.x = newVelocity;
        }
    }

}