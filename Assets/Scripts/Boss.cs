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
    public Vector3 offset;

    AudioSource audioSource;
    Animator animator;

    public List<AudioClip> hurtSounds;
    public AudioClip deathSound;

    float health;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        StartCoroutine(blink());
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

    IEnumerator blink() {
        float wait = Random.Range(2, 8);
        yield return new WaitForSeconds(wait);
        animator.SetTrigger("Blink");
        StartCoroutine(blink());
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
        int sign = -1;
        for (int i = 0; i < attack.burstAmount; i++)
        {
            if (attack.switchEvery > 0 && i % attack.switchEvery == 0) {
                sign = -sign;
            }

            if (health <= 0)
                break;
            
            if (i == 0 || attack.recalculateAngle)
            {
                angle = Random.Range(0, 360);
                if (attack.pattern == BulletPattern.TargetedSpread)
                {
                    Vector2 direction = Player.Instance.rb.position - (Vector2)(transform.position + offset);
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
                    if (attack.randomRotation) {
                        curAngle = Random.Range(angle - attack.spreadDelta, angle + attack.spreadDelta);
                    }
                    Vector3 position = transform.position + offset +
                        new Vector3(
                            Mathf.Cos(curAngle * Mathf.Deg2Rad),
                            Mathf.Sin(curAngle * Mathf.Deg2Rad),
                            0) * spriteRadius;
                    Quaternion rotation = Quaternion.Euler(0, 0, (attack.sameRotation ? angle : curAngle) - 90);
                    Projectile projectile = Instantiate(attack.projectilePrefab, position, rotation).GetComponent<Projectile>();
                    projectile.damage = attack.damage;
                    projectile.speed = attack.speed;
                    curAngle += delta * sign;
                }
            }
            angle += attack.burstAngleDelta * sign;
            yield return new WaitForSeconds(attack.burstDelay);
        }
        yield return new WaitForSeconds(attack.waitAfter);
    }

    public IEnumerator mainLoop()
    {
        yield return new WaitForSeconds(2);
        
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