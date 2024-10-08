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

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.up = mousePos - transform.position;
    }

    public void Shoot(int bullets)
    {
        float angle = -(bullets - 1) / 2 * angleDelta;
        for (int i = 0; i < bullets; i++)
        {
            float distance = Random.Range(minRange, maxRange);
            Transform bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation).transform;
            bullet.Rotate(new Vector3(0, 0, angle));

            float bulletSpeed = Random.Range(minSpeed, maxSpeed);
            bullet.DOMove(bullet.position + bullet.up * distance, bulletSpeed).SetSpeedBased(true).onComplete = () => { Destroy(bullet.gameObject); };
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, transform.up, distance);
            if (hit.collider != null && hit.collider.TryGetComponent(out Boss boss))
                boss.Health -= damage;
            angle += angleDelta;
        }
    }
}
