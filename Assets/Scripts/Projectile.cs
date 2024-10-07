using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 0;
    public float damage = 10;
    public float lifetime = 10;

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
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Player player))
        {
            player.Health -= damage;
            Destroy(gameObject);
        }
    }
}
