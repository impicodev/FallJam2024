using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RangedMinion : Minion
{
    public GameObject ProjectilePrefab;
    public float ProjectileDamage = 10.0f;
    public AnimationCurve ProjectileSpeed;
    public float ProjectileSpawnDistance = 0.65f;

    protected override void Follow()
    {
        if (Player.Instance == null || frozen)
        {
            activityTime = 0.0f;
            return;
        }

        Vector3 followPosition = Player.Instance.transform.position;

        body.MovePosition(Vector2.MoveTowards(body.position, followPosition, Speed * Time.deltaTime));

        if (activityTime >= AttackCooldown)
            SetActivity(ActivityState.Attacking);
    }

    protected override void StartAttack()
    {
        if (Player.Instance == null || frozen) return;

        Vector3 gap = Player.Instance.transform.position - transform.position;
        float projectileAngle = -Vector2.SignedAngle(gap, Vector2.up);

        Quaternion projectileRotation = Quaternion.Euler(0.0f, 0.0f, projectileAngle);

        Vector3 projectilePosition = transform.position + gap.normalized * ProjectileSpawnDistance;

        Projectile projectile = Instantiate(ProjectilePrefab, projectilePosition, projectileRotation).GetComponent<Projectile>();
        projectile.damage = ProjectileDamage;
        projectile.speed = ProjectileSpeed;
    }
}
