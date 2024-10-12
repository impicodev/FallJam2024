using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Minion : MonoBehaviour
{
    protected const float damageCooldown = 1.0f;

    public float Health = 10.0f;
    public float Speed = 2.0f;
    public float SpawnDazeDuration = 1.0f;
    public float DamageStunDuration = 0.2f;
    public float DeathDuration = 0.5f;
    public float PauseBeforeAttack = 0.1f;
    public float AttackDuration = 0.25f;
    public float PauseAfterAttack = 0.1f;
    public float AttackCooldown = 1.5f;
    public float ContactDamage = 10.0f;

    protected enum ActivityState {SpawnDazed, Following, Attacking, Stunned, Dying}

    protected ActivityState activity = ActivityState.SpawnDazed;
    protected Rigidbody2D body = null;
    protected float activityTime = 0.0f;
    protected float damageTime = damageCooldown;
    protected static bool frozen = false;
    private bool startedAttacking = false;
    private bool finishedAttacking = false;

    public static void SetFrozen(bool val)
    {
        frozen = val;
    }

    public void TakeDamage(float amount = 0.0f)
    {
        Health -= amount;
        Debug.Log("OWW. " + Health.ToString());

        if (Health <= 0.0)
            SetActivity(ActivityState.Dying);
        else
            SetActivity(ActivityState.Stunned);
    }

    protected virtual void Start()
    {
        Player.aliveMinions += 1;
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {   
        activityTime += Time.deltaTime;
        
        if (damageTime < damageCooldown)
            damageTime += Time.deltaTime; 

        switch (activity)
        {
            case ActivityState.SpawnDazed:
                Daze();
                break;
            
            case ActivityState.Following:
                Follow();
                break;
            
            case ActivityState.Attacking:
                Attack();
                break;

            case ActivityState.Stunned:
                Stun();
                break;
            
            case ActivityState.Dying:
                Die();
                break;

            default:
                break;
        }
    }

    private void Daze()
    {
        if (activityTime >= SpawnDazeDuration)
            SetActivity(ActivityState.Following);
    }

    protected virtual void Follow()
    {
        activityTime = 0.0f;

        if (Player.Instance == null || frozen) return;

        Vector3 followPosition = Player.Instance.transform.position;

        body.MovePosition(Vector2.MoveTowards(body.position, followPosition, Speed * Time.deltaTime));  
    }

    private void Attack()
    {
        if (activityTime >= PauseBeforeAttack && !startedAttacking)
        {
            StartAttack();
            
            activityTime = 0.0f;
            startedAttacking = true;
        }
        else if (activityTime >= AttackDuration && startedAttacking && !finishedAttacking)
        {
            FinishAttack();
            
            activityTime = 0.0f;
            finishedAttacking = true;
        }
        else if (activityTime >= PauseAfterAttack && finishedAttacking)
        {
            startedAttacking = false;
            finishedAttacking = false;
            SetActivity(ActivityState.Following);
        }
    }

    protected virtual void StartAttack() { }

    protected virtual void FinishAttack() { }

    private void Stun()
    {
        if (activityTime >= DamageStunDuration)
            SetActivity(ActivityState.Following);
    }

    private void Die()
    {
        if (activityTime > DeathDuration)
        {
            Destroy(gameObject);
            if (Player.Instance != null) Player.aliveMinions -= 1;
        }
    }

    protected void SetActivity(ActivityState val)
    {
        activity = val;
        activityTime = 0.0f;
    }

    protected virtual void HitPlayer(Player player)
    {
        if (damageTime >= damageCooldown)
        {
            player.Health -= ContactDamage;
            damageTime = 0.0f;
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            HitPlayer(player);
        }
    }
}
