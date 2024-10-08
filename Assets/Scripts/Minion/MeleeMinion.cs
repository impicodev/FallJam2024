using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MeleeMinion : Minion
{
    public GameObject MinionHitObject;
    public float AttemptAttackDistance = 1f;
    public float HitDistance = 1f;

    private MinionHit hit;
    private Transform hitTransform;
    private Collider2D hitCollider;

    private void Start()
    {
        hit = MinionHitObject.GetComponent<MinionHit>();
        hitTransform = MinionHitObject.GetComponent<Transform>();
        hitCollider = MinionHitObject.GetComponent<Collider2D>();
    }

    protected override void Follow()
    {
        Vector3 followPosition = Player.Instance.transform.position;

        transform.position = Vector2.MoveTowards(transform.position, followPosition, Speed * Time.deltaTime);

        float distance = (followPosition - transform.position).magnitude;

        if (distance <= AttemptAttackDistance)
            SetActivity(ActivityState.Attacking);
    }

    protected override void StartAttack()
    {
        hitTransform.localPosition = (Player.Instance.transform.position - transform.position).normalized * HitDistance;
        hitCollider.enabled = true;
    }

    protected override void FinishAttack()
    {
        hitCollider.enabled = false;
        hitTransform.localPosition = Vector2.zero;
    }

    protected override void HitPlayer(Player player)
    {
        if (damageTime >= damageCooldown && hit.CanDamage())
        {
            player.Health -= ContactDamage;
            damageTime = 0.0f;
            hit.ResetDamageCooldown();
        }
    }
}
