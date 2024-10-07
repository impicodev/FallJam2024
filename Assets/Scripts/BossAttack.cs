using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletPattern
{
    Radial,
    TargetedSpread,
    MinionSpawn
}

[System.Serializable]
public class BossAttack
{
    public GameObject projectilePrefab;
    public BulletPattern pattern;
    [Header("Projectile Fields")]
    [Tooltip("Quantity of projectiles / minions")]
    public int amount = 1;
    [Tooltip("Projectile damage")]
    public float damage = 10;
    [Tooltip("Projectile speed")]
    public float speed = 5;
    [Header("Spread Fields")]
    [Tooltip("Angle delta between projectiles")]
    public float spreadDelta = 0;
    [Tooltip("Determines whether the bullets will share a common rotation, or angle outwards")]
    public bool sameRotation = false;
    [Header("Attack Fields")]
    [Tooltip("Probability that attack will be chosen relative to others; irrelevant if attacks are ordered")]
    public float likelihood = 10;
    public float waitTime = 0;
    public int burstAmount = 1;
    public float burstDelay = 0.3f;
    [Tooltip("Angle change between burst shots")]
    public float burstAngleDelta = 5;
}
