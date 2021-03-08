using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PhysicsEntity
{
    [SerializeField] float stompDamage;
    [SerializeField] float StaffDamage = 5;
    Rigidbody2D rb2d;
    SpriteRenderer spriteRenderer;
    Health health;
    Animator animator;

    //StateMachine
    StateMachine stateMachine;

    [SerializeField] Stomper stomper;

    const int StNormal = 0;
    const int StHanging = 1;
    const int StImpulsed = 2;
    const int StAttacking = 3;
    const int StateNumber = 4;

    //Ground detection
    bool isGrounded;
    float groundedCounter;
    [SerializeField] float groundedTime = 0.1f;
    private float airDragCoeficient = 0.6f;
    private float groundDragCoeficient = 0.7f;

    //crossState
    [SerializeField] float runSpeed = 7, jumpSpeed = 9, boostedJump = 20, attackImpulseSpeed = 9;
    [SerializeField] private float knockbackSpeed = 5;
    [SerializeField] float attackUpwardImpulse = 6;
    float jumpBufferCounter; // top allow store jump input before hitting ground 
    [SerializeField] float jumpBufferTime = 0.05f;
    private bool impulseHasMomentum;
    bool rotationLocked;

    Collider2D mainCollider;
    private bool canAttack = true;
    private bool activelyMoving;
    //Normal

    //Hanging
    Vector2 hangRaycastOffset = new Vector2(0, 0.1f);
    Vector2 handPositionHangingRight = new Vector2(0.325f, 0.17f);
    Vector2 handPositionHangingLeft = new Vector2(-0.325f, 0.17f);
    private const float hangDistance = 0.2f;
    private const float hangCooldownTime = 0.5f;
    float hangCooldown;
    private Bounds? _hangBlockBounds;

    //Impulsed
    private Vector2 impulse;
    private float impulseCountDown;
    [SerializeField] private float impulseTime = 0.2f;

    //Attacking
    private float attackAnimationTime;
    const float AttackAnimationDuration = 0.25f;
    Vector2 AttackDirection;
    bool canHitImpulse;
    const float attackNormalMaxAngle = 45;


    #region Normal State

    private int NormalUpdate()
    {
        EvaluateHorizontalControl();

        EvaluateJump();

        if (attackCommand())
        {
            return StAttacking;
        }

        if (CanHang())
        {
            return StHanging;
        }


        return StNormal;
    }

    #endregion

    #region HangingState
    int HangingUpdate()
    {
        if (jumpBufferCounter > 0)
        {
            if (!(Input.GetAxis("Vertical") < 0))
            {
                Speed.y = jumpSpeed;
            }
            return StNormal;
        }

        return StHanging;
    }

    void HangingBegin()
    {
        Speed = Vector2.zero;
        animator.SetBool("Hanging", true);

        if (_hangBlockBounds == null) return;

        Bounds hangBlockBounds = _hangBlockBounds.Value;
        Vector2 hangingCorner;
        Vector2 handPosition;

        if (facingDirection == FacingRight)
        {
            hangingCorner = new Vector2(hangBlockBounds.min.x, hangBlockBounds.max.y);
            handPosition = (Vector2)transform.position + handPositionHangingRight;
        }
        else
        {
            hangingCorner = hangBlockBounds.max;
            handPosition = (Vector2)transform.position + handPositionHangingLeft;
        }

        transform.Translate(hangingCorner - handPosition);

    }

    void HangingEnd()
    {
        animator.SetBool("Hanging", false);
        hangCooldown = hangCooldownTime;
    }

    bool CanHang()
    {
        if (isGrounded || Speed.y > 0) return false;
        if (hangCooldown > 0) return false;

        Vector2 position;
        Vector2 direction;

        if (facingDirection == FacingLeft)
        {
            position = (Vector2)transform.position + handPositionHangingLeft;
            direction = Vector2.left;
        }
        else
        {
            position = (Vector2)transform.position + handPositionHangingRight;
            direction = Vector2.right;
        }

        RaycastHit2D grabBlockHit = Physics2D.Raycast(position - hangRaycastOffset, direction, hangDistance, SolidLayer);
        RaycastHit2D freeSpaceHit = Physics2D.Raycast(position + hangRaycastOffset, direction, hangDistance, SolidLayer);

        bool hangBlockIsSolid = grabBlockHit.collider != null;
        bool upperSpaceOcupied = freeSpaceHit.collider != null;

        if (hangBlockIsSolid)
        {
            Vector2 blockPosition = position - hangRaycastOffset + direction * hangDistance;
            _hangBlockBounds = grabBlockHit.collider.bounds;
            _hangBlockBounds = MapCoordenates.GetCellBounds(blockPosition);
        }

        return hangBlockIsSolid && !upperSpaceOcupied;
    }

    #endregion

    #region Attack

    private bool attackCommand()
    {
        return (Input.GetButtonDown("Attack Normal") || Input.GetButtonDown("Attack Up") || Input.GetButtonDown("Attack Down")) && canAttack;
    }

    private void AttackBegin()
    {
        attackAnimationTime = AttackAnimationDuration;
        canHitImpulse = true;
        rotationLocked = true;
        if (Input.GetButton("Attack Up")) AttackUp();
        else if (Input.GetButton("Attack Down")) AttackDown();
        else if (facingDirection == FacingLeft) AttackLeft();
        else AttackRight();
    }

    int AttackUpdate()
    {
        EvaluateHorizontalControl();
        EvaluateJump();
        attackAnimationTime -= Time.deltaTime;
        if (attackAnimationTime < 0)
        {
            return StNormal;
        }
        return StAttacking;
    }

    void AttackEnd()
    {
        canHitImpulse = false;
        rotationLocked = false;
        animator.ResetTrigger("AttackUp");
        animator.ResetTrigger("AttackLeft");
        animator.ResetTrigger("AttackRight");
        animator.ResetTrigger("AttackDown");
    }

    private void AttackUp()
    {
        AttackDirection = Vector2.up;
        animator.SetTrigger("AttackUp");
    }
    private void AttackLeft()
    {
        AttackDirection = Vector2.left;
        animator.SetTrigger("AttackLeft");
    }
    private void AttackRight()
    {
        AttackDirection = Vector2.right;
        animator.SetTrigger("AttackRight");
    }

    private void AttackDown()
    {
        AttackDirection = Vector2.down;
        animator.SetTrigger("AttackDown");
    }

    void ObstacleHit(Vector2 normal)
    {
        if (isGrounded || !canHitImpulse) return;

        float angleAttackNormal = Mathf.Abs(Vector2.Angle(normal, -AttackDirection));

        if (angleAttackNormal > attackNormalMaxAngle) return;

        canHitImpulse = false;
        Vector2 impulse;

        impulse = -AttackDirection * attackImpulseSpeed;

        if (AttackDirection.y == 0) //horizontal hits get a up impulse
        {
            impulse.y = attackUpwardImpulse;
        }
        else if (AttackDirection.y < 0 && Speed.y > 0)
        {
            impulse.y = boostedJump;
        }

        stateMachine.State = StartImpulse(impulse);

    }

    #endregion

    #region Impulse
    private int StartImpulse(Vector2 impulse)
    {
        this.impulse = impulse;
        return StImpulsed;
    }

    void ImpulseBegin()
    {
        impulseCountDown = impulseTime;
        impulseHasMomentum = true;

        if (impulse.y != 0)
        {
            Speed.y = impulse.y;
        }

        if (impulse.x != 0)
        {
            Speed.x = impulse.x;
        }

    }
    int ImpulseUpdate()
    {
        impulseCountDown -= Time.deltaTime;

        if (impulseCountDown < 0)
        {
            return StNormal;
        }

        return StImpulsed;
    }



    #endregion

    #region Global State methods


    internal void TakeDamage(float amount, Vector3 damageSourcePosition)
    {
        health.TakeDamage(amount);
        Vector3 knockbackRelativePosition = transform.position - damageSourcePosition;
        float horizontalDirection = Mathf.Sign(knockbackRelativePosition.x);
        Vector2 knockbackDirection = Vector2.up + Vector2.right * horizontalDirection;
        stateMachine.State = StartImpulse(knockbackDirection.normalized * knockbackSpeed);
        animator.SetTrigger("Damaged");

    }

    private void EvaluateHorizontalControl()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            activelyMoving = true;
            Speed.x = runSpeed;
            impulseHasMomentum = false;
            if (!rotationLocked) facingDirection = FacingRight;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            activelyMoving = true;
            Speed.x = -runSpeed;
            impulseHasMomentum = false;
            if (!rotationLocked) facingDirection = FacingLeft;
        }
        else
        {
            activelyMoving = false;
        }
    }

    private void EvaluateJump()
    {
        if (jumpBufferCounter > 0 && groundedCounter > 0)
        {
            jumpBufferCounter = 0;
            groundedCounter = 0;

            if (Input.GetButton("Jump"))
                Speed.y = jumpSpeed;
            else
                Speed.y = jumpSpeed * 0.5f;//avoid doing full jump with buffered input 
        }

        if (Input.GetButtonUp("Jump") && Speed.y > 0)
        {
            Speed.y = Speed.y * 0.5f;
        }
    }

    protected bool CheckGround()
    {
        float heightCheck = 0.05f;
        float widthReduction = 0.8f; //to avoid detecting ground in walls;

        Vector2 lowCenterPosition = new Vector2(_mainCollider.bounds.center.x, _mainCollider.bounds.min.y);
        lowCenterPosition.y += heightCheck / 2;

        Vector2 boxSize = new Vector2(_mainCollider.bounds.size.x * widthReduction, heightCheck);

        RaycastHit2D raycastHit = Physics2D.BoxCast(lowCenterPosition,
                                                    boxSize,
                                                    0f, Vector2.down, heightCheck,
                                                    SolidLayer);

        ExtDebug.DrawBoxCast2D(lowCenterPosition,
        boxSize,
        0f, Vector2.down, heightCheck,
        Color.yellow);
        return raycastHit.collider != null;
    }

    float GetGroundDrag()
    {
        return groundDragCoeficient; //different ground can have diferent drags;
    }

    void OnStomp()
    {
        print("Stomp");
        Speed.y = jumpSpeed;
        return;
    }
    #endregion

    #region Core methods
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine(StateNumber);

        stateMachine.SetCallbacks(StNormal, NormalUpdate, null, null, null);
        stateMachine.SetCallbacks(StHanging, HangingUpdate, null, HangingBegin, HangingEnd);
        stateMachine.SetCallbacks(StImpulsed, ImpulseUpdate, null, ImpulseBegin, null);
        stateMachine.SetCallbacks(StAttacking, AttackUpdate, null, AttackBegin, AttackEnd);

        stomper.onStomp += OnStomp;
        StaffHitter.onObstacleHit += ObstacleHit;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        _mainCollider = GetComponent<Collider2D>();
        health = GetComponent<Health>();

    }

    new void Update()
    {
        jumpBufferCounter -= Time.deltaTime;
        groundedCounter -= Time.deltaTime;
        hangCooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        isGrounded = CheckGround();
        if (isGrounded)
        {
            groundedCounter = groundedTime;
            impulseHasMomentum = false;
            dragCoeficient = GetGroundDrag();
        }
        else
        {
            dragCoeficient = airDragCoeficient;
        }

        stateMachine.Update();

        IgnoreGravity = isGrounded || stateMachine.State == StHanging;
        IgnoreDrag = activelyMoving || stateMachine.State == StImpulsed || impulseHasMomentum;

        base.Update();

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        bool isWalking = activelyMoving && isGrounded;
        animator.SetFloat("SpeedY", Speed.y);
        animator.SetBool("Walking", isWalking);

        bool spriteNeedsFliping = facingDirection == FacingLeft;
        GetComponent<SpriteRenderer>().flipX = spriteNeedsFliping;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        DamageSource damageSource = other.gameObject.GetComponent<DamageSource>();
        if (damageSource != null)
        {
            TakeDamage(damageSource.Damage, other.transform.position);
        }
    }

    private void OnDisable()
    {
        stomper.onStomp -= OnStomp;
        StaffHitter.onObstacleHit -= ObstacleHit;
    }


    #endregion

}
