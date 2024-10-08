using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    //temporary, for testing purposes (to be replaced with player transform or position) 
    public Transform FollowTransform;
    
    public float Speed = 2.0f;
    public float SpawnDazeDuration = 1.0f;
    public float PauseBeforeAttack = 0.75f;
    public float PauseAfterAttack = 0.75f;

    protected enum ActivityState {SpawnDazed, Following, Attacking, Dying}

    protected ActivityState activity = ActivityState.SpawnDazed;
    protected float activityTime = 0.0f;

    protected void Update()
    {
        activityTime += Time.deltaTime;

        switch (activity)
        {
            case ActivityState.SpawnDazed:
                Daze();
                break;
            
            case ActivityState.Following:
                Follow();
                break;
            
            case ActivityState.Attacking:
                Attack();
                break;
            
            case ActivityState.Dying:
                break;

            default:
                break;
        }
    }

    protected void Daze()
    {
        if (activityTime >= SpawnDazeDuration)
            SetActivity(ActivityState.Following);
    }

    protected virtual void Follow() { }

    protected virtual void Attack() { }

    protected void SetActivity(ActivityState val)
    {
        activity = val;
        activityTime = 0;
    }
}
