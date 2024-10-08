using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 10;
    
    [System.NonSerialized] public float speed = 5;
    [System.NonSerialized] public float damage = 10;

    private void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        Move();
    }

    public virtual void Move()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }

    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.GetComponentInParent<Player>();
        if (player != null)
        {
            if(collider.name == "ParryTool") {
                player.Ammo += 1;
                Destroy(gameObject);
                return;
            }
            player.Health -= damage;
            Destroy(gameObject);
        }
    }
}
