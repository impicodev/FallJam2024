using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Gun : MonoBehaviour
{
    public float damage = 10;
    public float minRange = 2;
    public float maxRange = 3;
    public float angleDelta = 10;
    public float minSpeed = 10;
    public float maxSpeed = 12;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Animator animator;

    private void Update()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -Mathf.Sign(transform.parent.localScale.x);
        scale.x *= Mathf.Sign(transform.up.x);
        transform.localScale = scale;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.up = mousePos - transform.position;
    }

    public void Shoot(int bullets)
    {
        animator.SetTrigger("Shoot");
        float angle = -(bullets - 1) / 2 * angleDelta;
        for (int i = 0; i < bullets; i++)
        {
            float distance = Random.Range(minRange, maxRange);
            Transform bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation).transform;
            bullet.Rotate(new Vector3(0, 0, angle));

            float bulletSpeed = Random.Range(minSpeed, maxSpeed);
            bullet.DOMove(bullet.position + bullet.up * distance, bulletSpeed).SetSpeedBased(true).onComplete = () => { Destroy(bullet.gameObject); };
            RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, transform.up, distance);
            foreach (Collider2D collider in Physics2D.OverlapPointAll(firePoint.position))
            {
                if (collider.TryGetComponent(out Boss component))
                    component.Health -= damage;
                if (collider.TryGetComponent(out Minion _minion))
                    _minion.TakeDamage(damage);
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.TryGetComponent(out Boss boss))
                    boss.Health -= damage;
                if (hit.collider != null && hit.collider.TryGetComponent(out Minion minion))
                    minion.TakeDamage(damage);
            }
            angle += angleDelta;
        }
    }
}
