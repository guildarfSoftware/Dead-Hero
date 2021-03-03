using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Health))]
public class Enemy : PhysicsEntity, IHitable
{
    GameObject player;
    Health health;
    //State Machine
    StateMachine stateMachine;
    const int StWalking = 0;
    const int StAtacking = 1;
    const int StHit = 2;
    const int StDead = 3;
    const int StateNumber = 4;


    //Cross State 
    const float spriteOffset = 0.6f;
    private bool canAttackPlayer;
    private Vector2 playerRelativePosition;

    //Walk state
    private float walkSpeed = 3;
    private bool stuck;
    float colliderSizeMargin = 0.1f;

    //Attack State
    float attackAnimationTime;
    const float attackAnimationDuration = 0.7f;
    private const int SnakeDamage = 1;

    //Hit state
    private float hitAnimationCountdown;
    private float hitAnimationTime = 0.5f;

    //Dead State
    float deathAnimationCountdown;
    float deathAnimationTime = 0.5f;
    [SerializeField] Collider2D stompCollider;

    void Start()
    {
        health = GetComponent<Health>();
        _mainCollider = GetComponent<Collider2D>();
        stateMachine = new StateMachine(StateNumber);
        stateMachine.SetCallbacks(StWalking, WalkUpdate, null, WalkBegin, WalkEnd);
        stateMachine.SetCallbacks(StAtacking, AttackUpdate, null, AttackBegin, null);
        stateMachine.SetCallbacks(StHit, HitUpdate, null, HitBegin, null);
        stateMachine.SetCallbacks(StDead, DeathUpdate, null, DeathBegin, null);
        stateMachine.State = StWalking;
        StartCoroutine(stateMachine.CoroutineHandler());
        player = GameObject.FindGameObjectWithTag("Player");
        IgnoreDrag = true;
        IgnoreGravity = true;
    }
    #region CrossState Methods

    public void Hit(float amount)
    {
        health.TakeDamage(amount);

        if (health.IsDead)
        {
            stateMachine.State = StDead;
        }
        else
        {
            stateMachine.State = StHit;
        }
    }
    #endregion

    #region walk State
    void WalkBegin()
    {
        Vector2 position = transform.position;
        if (IsWalkable(position + Vector2.right * (_mainCollider.bounds.extents.x + colliderSizeMargin)))
        {
            facingDirection = FacingRight;
            Speed.x = walkSpeed;
        }
        else if (IsWalkable(position + Vector2.left * (_mainCollider.bounds.extents.x + colliderSizeMargin)))
        {
            facingDirection = FacingLeft;
            Speed.x = walkSpeed;
        }
        else
        {
            stuck = true;
            Speed.x = 0;
        }
    }
    int WalkUpdate()
    {
        if (canAttackPlayer) return StAtacking;

        if (stuck) return StWalking;

        Vector2 position = transform.position;
        if (!IsWalkable(position + Speed.normalized * (_mainCollider.bounds.extents.x + colliderSizeMargin)))
        {
            ChangeDirection();
            if (!IsWalkable(position + Speed.normalized * (_mainCollider.bounds.extents.x + colliderSizeMargin)))
            {
                stuck = true;
                Speed.x = 0;
            }
        }


        return StWalking;
    }

    void WalkEnd()
    {
        Speed.x = 0;
    }

    #endregion

    #region attack State
    int AttackUpdate()
    {
        attackAnimationTime -= Time.deltaTime;
        if (attackAnimationTime < 0) return stateMachine.PreviousState;
        return StAtacking;
    }

    void AttackBegin()
    {
        if (playerRelativePosition.x > 0)
        {
            facingDirection = FacingRight;
        }
        else
        {
            facingDirection = FacingLeft;
        }

        attackAnimationTime = attackAnimationDuration;

        GetComponent<Animator>().SetTrigger("Attack");
        Animator animator = GetComponent<Animator>();
        AnimatorClipInfo[] info = animator.GetNextAnimatorClipInfo(0);
        AnimatorClipInfo[] info2 = animator.GetCurrentAnimatorClipInfo(0);
        AnimatorStateInfo info3 = animator.GetNextAnimatorStateInfo(0);
    }


    #endregion

    #region hit State
    private int HitUpdate()
    {
        hitAnimationCountdown -= Time.deltaTime;
        if (hitAnimationCountdown < 0)
        {
            return StWalking;
        }
        return StHit;
    }

    private void HitBegin()
    {
        print("startHitAnim");//startDeath animation
        hitAnimationCountdown = hitAnimationTime;
    }
    #endregion

    #region dead State

    void DeathBegin()
    {
        print("startDeathAnim");//startDeath animation
        deathAnimationCountdown = deathAnimationTime;
    }
    int DeathUpdate()
    {
        deathAnimationCountdown -= Time.deltaTime;
        if (deathAnimationCountdown < 0)
        {
            Destroy(gameObject);
        }
        return StDead;
    }


    #endregion

    // Update is called once per frame
    new void Update()
    {
        stateMachine.Update();

        base.Update();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        bool isMoving = stateMachine.State == StWalking;
        GetComponent<Animator>().SetBool("Moving", isMoving);
        bool spriteNeedsFliping = facingDirection == FacingLeft;
        GetComponent<SpriteRenderer>().flipX = spriteNeedsFliping;

    }

    private void ChangeDirection()
    {
        facingDirection = facingDirection == FacingRight ? FacingLeft : FacingRight;
        Speed.x = -Speed.x;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.otherCollider == _mainCollider)   //player hit enemy body
            {
                if(health.IsDead) return;
                canAttackPlayer = true;
                playerRelativePosition = other.transform.position - transform.position;
            }
            else if (other.otherCollider == stompCollider) //player hit stompable area
            {
                health.TakeDamage(float.MaxValue);
                stateMachine.State = StDead;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.otherCollider == _mainCollider)   //player hit enemy body
            {
                canAttackPlayer = false;
            }
        }
    }

    bool IsWalkable(Vector2 position)
    {
        bool isObstructed = MapCoordenates.IsSolid(position);
        bool hasFloor = MapCoordenates.IsSolid(position + Vector2.down);

        return !isObstructed && hasFloor;
    }

}
