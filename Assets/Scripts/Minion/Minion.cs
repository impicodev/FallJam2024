using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Minion : MonoBehaviour
{
    protected const float damageCooldown = 1.0f;

    public float Speed = 2.0f;
    public float SpawnDazeDuration = 1.0f;
    public float PauseBeforeAttack = 0.1f;
    public float AttackDuration = 0.25f;
    public float PauseAfterAttack = 0.1f;
    public float AttackCooldown = 1.5f;
    public float ContactDamage = 10.0f;

    protected enum ActivityState {SpawnDazed, Following, Attacking, Dying}

    protected ActivityState activity = ActivityState.SpawnDazed;
    protected float activityTime = 0.0f;
    protected float damageTime = damageCooldown;
    private bool startedAttacking = false;
    private bool finishedAttacking = false;

    private void Update()
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
            
            case ActivityState.Dying:
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

    protected abstract void Follow();

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

    protected abstract void StartAttack();

    protected abstract void FinishAttack();

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

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            HitPlayer(player);
        }
    }
}
