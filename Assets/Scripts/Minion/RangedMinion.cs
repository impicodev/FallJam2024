using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RangedMinion : Minion
{
    public GameObject ProjectilePrefab;
    public float ProjectileDamage = 10.0f;
    public float ProjectileSpeed = 5.0f;
    public float ProjectileSpawnDistance = 0.65f;

    protected override void Follow()
    {
        Vector3 followPosition = Player.Instance.transform.position;

        transform.position = Vector2.MoveTowards(transform.position, followPosition, Speed * Time.deltaTime);

        if (activityTime >= AttackCooldown)
            SetActivity(ActivityState.Attacking);
    }

    protected override void StartAttack()
    {
        Vector3 gap = Player.Instance.transform.position - transform.position;
        float projectileAngle = -Vector2.SignedAngle(gap, Vector2.up);

        Quaternion projectileRotation = Quaternion.Euler(0.0f, 0.0f, projectileAngle);

        Vector3 projectilePosition = transform.position + gap.normalized * ProjectileSpawnDistance;

        Projectile projectile = Instantiate(ProjectilePrefab, projectilePosition, projectileRotation).GetComponent<Projectile>();
        projectile.damage = ProjectileDamage;
        projectile.speed = ProjectileSpeed;
    }

    protected override void FinishAttack()
    {
        
    }
}
