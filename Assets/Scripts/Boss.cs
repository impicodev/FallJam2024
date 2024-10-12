using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    public float Health
    {
        get { return health; }
        set
        {
            if(value - health < 0) {
                PlayHurtSound();
            }
            health = value;

            // update healthbar UI
            //Debug.Log("Boss is at " + health + " health");
            manager.DisplayBossHealth(health / maxHealth);

            if (health <= 0)
            {
                audioSource.clip = deathSound;
                audioSource.Play();
                enabled = false;
                manager.BossDied();
            }
        }
    }

    public string Name = "Bossy";
    public List<BossAttack> attacks;
    public bool attacksAreOrdered = false;
    public float maxHealth = 100;
    public float spriteRadius = 1;
    public BossSceneManager manager;

    AudioSource audioSource;

    public List<AudioClip> hurtSounds;
    public AudioClip deathSound;

    float health;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void FlashHurt()
    {
        sprite.color = Color.red;
        DOTween.To(() => sprite.color, x => sprite.color = x, Color.white, 0.5f);
    }

    private void PlayHurtSound() {
        int i = (int) Random.Range(0, hurtSounds.Count);
        audioSource.clip = hurtSounds[i];
        audioSource.Play();
    }

    public void BeginAttacking()
    {
        Health = maxHealth;
        StartCoroutine(mainLoop());
        Debug.Log("begun attacking");
    }

    public IEnumerator attack(BossAttack attack)
    {
        yield return new WaitForSeconds(attack.waitBefore);
        float angle = 0;
        for (int i = 0; i < attack.burstAmount; i++)
        {
            if (health <= 0)
                break;
            
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
                for (int j = 0; j < attack.amount; j++)
                {
                    Vector3 corner = Camera.main.ScreenToWorldPoint(Vector3.zero);
                    Vector3 point = new Vector3(Random.Range(corner.x, -corner.x), Random.Range(corner.y, -corner.y), 0);
                    Instantiate(attack.projectilePrefab, point, Quaternion.identity);
                }
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
        yield return new WaitForSeconds(attack.waitAfter);
    }

    public IEnumerator mainLoop()
    {
        Debug.Log("main looping");
        
        int i = -1;
        while (health > 0)
        {
            if (attacksAreOrdered)
            {
                i = (i + 1) % attacks.Count;
            }
            else
            {
                //Debug.Log(Player.aliveMinions);

                i = attacks.Count - 1;

                float sum = 0;
                foreach (BossAttack attack in attacks)
                {
                    float likelihood = attack.likelihood;
                    if (attack.pattern == BulletPattern.MinionSpawn)
                        likelihood /= Player.aliveMinions + 1;
                    sum += attack.likelihood;
                }

                float point = Random.Range(0, sum);
                for (int j = 0; j < attacks.Count; j++)
                {
                    float likelihood = attacks[j].likelihood;
                    if (attacks[j].pattern == BulletPattern.MinionSpawn)
                        likelihood /= Player.aliveMinions + 1;

                    point -= likelihood;
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
}