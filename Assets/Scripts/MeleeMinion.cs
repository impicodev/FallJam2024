using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMinion : Minion
{
    public Transform HitTransform;
    public Collider2D HitCollider;
    public float AttemptAttackDistance = 1f;
    public float HitDistance = 1f;

    private bool hasAttacked = false;

    protected override void Follow()
    {
        if (FollowTransform != null)
            transform.position = Vector2.MoveTowards(transform.position, FollowTransform.position, Speed * Time.deltaTime);

        float distance = (FollowTransform.position - transform.position).magnitude;

        if (distance <= AttemptAttackDistance)
            SetActivity(ActivityState.Attacking);
    }

    protected override void Attack()
    {
        if (activityTime >= PauseBeforeAttack && !hasAttacked)
        {
            HitTransform.localPosition = (FollowTransform.position - transform.position).normalized * HitDistance;
            HitCollider.enabled = true;

            Debug.Log("attack");
            activityTime = 0.0f;
            hasAttacked = true;
        }
        else if (activityTime >= PauseAfterAttack && hasAttacked)
        {
            HitCollider.enabled = false;
            HitTransform.localPosition = Vector2.zero;
            
            SetActivity(ActivityState.Following);
            hasAttacked = false;
        }
    }
}
