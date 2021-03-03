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

    RaycastHit2D[] hits = new RaycastHit2D[16];
    public const Facing FacingRight = Facing.right;
    public const Facing FacingLeft = Facing.left;
    protected Facing facingDirection;
    protected Vector2 Speed;
    protected bool IgnoreGravity, IgnoreDrag;

    void Move(Vector2 position)
    {
        _mainCollider = GetComponent<Collider2D>();
        int nHits = Physics2D.BoxCastNonAlloc(_mainCollider.bounds.center, _mainCollider.bounds.size, 0, position.normalized, hits, position.magnitude, SolidLayer);

        RaycastHit2D? closestCollision = null;
        for (int i = 0; i < nHits; i++)
        {
            RaycastHit2D hit = hits[i];
            if (closestCollision == null || closestCollision.Value.distance > hit.distance)
            {
                closestCollision = hit;
            }
        }
        if (closestCollision != null)
        {
            if (closestCollision.Value.normal.x != 0)
            {
                Speed.x = 0;
            }
            if (closestCollision.Value.normal.y != 0)
            {
                Speed.y = 0;
            }
            Bounds b = closestCollision.Value.collider.bounds;
            ExtDebug.DrawBoxCast2D(b.center, b.extents, 0, Vector2.zero, 0, Color.red);
        }
        transform.Translate(Speed * Time.deltaTime);
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