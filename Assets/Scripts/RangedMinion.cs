using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinion : Minion
{
    public float AttackCooldown = 5.0f;

    private bool hasAttacked = false;

    protected override void Follow()
    {
        if (FollowTransform != null)
            transform.position = Vector2.MoveTowards(transform.position, FollowTransform.position, Speed * Time.deltaTime);

        if (activityTime >= AttackCooldown)
            SetActivity(ActivityState.Attacking);
    }

    protected override void Attack()
    {
        if (activityTime >= PauseBeforeAttack && !hasAttacked)
        {
            //Attack
            Debug.Log("attack");
            activityTime = 0.0f;
            hasAttacked = true;
        }
        else if (activityTime >= PauseAfterAttack && hasAttacked)
        {
            SetActivity(ActivityState.Following);
            hasAttacked = false;
        }
    }

}
