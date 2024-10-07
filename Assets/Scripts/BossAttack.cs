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
    public float likelihood = 10;
    public float waitTime = 0;
    public int burstAmount = 1;
    public float burstDelay = 0.3f;
    public float burstAngleDelta = 5;
}
