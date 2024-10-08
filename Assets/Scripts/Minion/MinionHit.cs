using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionHit : MonoBehaviour
{
    private const float damageCooldown = 1.0f;

    public float HitDamage = 10.0f;

    private float damageTime = damageCooldown;

    private void Update()
    {
        if (damageTime < damageCooldown)
            damageTime += Time.deltaTime;
    }

    public void ResetDamageCooldown()
    {
        damageTime = 0.0f;
    }

    public bool CanDamage()
    {
        return (damageTime >= damageCooldown);
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out Player player))
        {
            if (damageTime >= damageCooldown)
            {
                player.Health -= HitDamage;
                damageTime = 0.0f;
            }
        }
    }
}
