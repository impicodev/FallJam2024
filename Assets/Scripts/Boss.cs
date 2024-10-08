using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            // update healthbar UI
            Debug.Log("Boss is at " + health + " health");
            if (health <= 0)
                Die();
        }
    }

    public List<BossAttack> attacks;
    public bool attacksAreOrdered = false;
    public float maxHealth = 100;
    public float spriteRadius = 1;

    float health;

    private void Start()
    {
        health = maxHealth;
        StartCoroutine(mainLoop());
    }

    public IEnumerator attack(BossAttack attack)
    {
        yield return new WaitForSeconds(attack.waitTime);
        float angle = 0;
        for (int i = 0; i < attack.burstAmount; i++)
        {
            if (i == 0 || attack.recalculateAngle)
            {
                angle = Random.Range(0, 360);
                if (attack.pattern == BulletPattern.TargetedSpread)
                {
                    Vector2 direction = Player.Instance.transform.position - transform.position;
                    angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                }
            }

            if (attack.pattern == BulletPattern.MinionSpawn)
            {
                // TODO
            }
            else if (attack.pattern == BulletPattern.Radial || attack.pattern == BulletPattern.TargetedSpread)
            {
                float curAngle = angle;
                float delta;
                if (attack.pattern == BulletPattern.Radial)
                    delta = 360 / attack.amount;
                else
                {
                    delta = attack.spreadDelta;
                    curAngle -= (attack.amount - 1) / 2 * delta;
                }

                for (int j = 0; j < attack.amount; j++)
                {
                    Vector3 position = transform.position +
                        new Vector3(
                            Mathf.Cos(curAngle * Mathf.Deg2Rad),
                            Mathf.Sin(curAngle * Mathf.Deg2Rad),
                            0) * spriteRadius;
                    Quaternion rotation = Quaternion.Euler(0, 0, (attack.sameRotation ? angle : curAngle) - 90);
                    Projectile projectile = Instantiate(attack.projectilePrefab, position, rotation).GetComponent<Projectile>();
                    projectile.damage = attack.damage;
                    projectile.speed = attack.speed;
                    curAngle += delta;
                }
            }
            angle += attack.burstAngleDelta;
            yield return new WaitForSeconds(attack.burstDelay);
        }
    }

    public IEnumerator mainLoop()
    {
        int i = -1;
        while (true)
        {
            if (attacksAreOrdered)
            {
                i = (i + 1) % attacks.Count;
            }
            else
            {
                float sum = 0;
                foreach (BossAttack attack in attacks)
                    sum += attack.likelihood;

                float point = Random.Range(0, sum);
                for (int j = 0; j < attacks.Count; j++)
                {
                    point -= attacks[j].likelihood;
                    if (point <= 0)
                    {
                        i = j;
                        break;
                    }
                }
            }

            yield return StartCoroutine(attack(attacks[i]));
        }
    }

    private void Die()
    {
        // TODO
    }
}