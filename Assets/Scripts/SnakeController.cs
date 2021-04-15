using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Health))]
public class SnakeController : PhysicsEntity, IHitable, IStompable
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

    public void Stomped(float stompDamage)
    {
        stateMachine.State = StDead;
    }

    #endregion

    #region walk State
    void WalkBegin()
    {
        Vector2 movingDirection = facingDirection == FacingRight ? Vector2.right : Vector2.left;

        Speed.x = movingDirection.x * walkSpeed;

    }

    int WalkUpdate()
    {
        if (canAttackPlayer) return StAtacking;

        if (stuck) return StWalking;
        Vector3 movingDirection = Speed.normalized;

        if (!IsWalkable(transform.position + movingDirection))
        {
            if (IsWalkable(transform.position - movingDirection))
            {
                ChangeDirection();
            }
            else
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

    bool IsWalkable(Vector3 position)
    {
        bool isObstructed = MapCoordenates.IsSolid(position);
        bool hasFloor = MapCoordenates.IsSolid(position + Vector3.down);

        return !isObstructed && hasFloor;
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
        player.GetComponent<PlayerController>().TakeDamage(1, transform.position);
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
        print("startHitAnim");// animation
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

    #region Core Methods
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (health.IsDead) return;
            canAttackPlayer = true;
            playerRelativePosition = other.transform.position - transform.position;
        }

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            canAttackPlayer = false;
        }

    }
    #endregion

}
