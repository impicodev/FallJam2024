using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public List<BossAttack> attacks;
    public bool attacksAreOrdered = false;

    public IEnumerator attack(BossAttack attack)
    {
        yield return new WaitForSeconds(attack.waitTime);
        float angle = Random.Range(0, 360);
        if (attack.pattern == BulletPattern.TargetedSpread)
        {
            //           TODO (player position)
            Vector2 direction = Vector3.zero - transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        for (int i = 0; i < attack.burstAmount; i++)
        {
            // TODO (attack)
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

    private void Start()
    {
        StartCoroutine(mainLoop());
    }
}