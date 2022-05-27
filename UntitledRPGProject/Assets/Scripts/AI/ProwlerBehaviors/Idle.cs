using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : P_State
{
    GameObject player;
    float mTime = 0.0f;
    public override void Enter(Prowler agent)
    {
        mTime = 0.0f;
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    public override void Execute(Prowler agent)
    {
        mTime += Time.deltaTime;
        if (mTime > agent.mStandbyTime)
        {
            if((agent.GetType().Name == "EnemyProwler"))
            {
                agent.ChangeBehavior("Find");
            }
        }
    }

    public override void Exit(Prowler agent)
    {
    }
}
