using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : P_State
{
    float mTime = 0.0f;
    public override void Enter(Prowler agent)
    {
        mTime = 0.0f;
        agent.mAgent.velocity = Vector3.zero;
    }

    public override void Execute(Prowler agent)
    {
        agent.mAgent.velocity = Vector3.zero;
        mTime += Time.deltaTime;
        if (mTime > agent.mStandbyTime)
        {
            if((agent.GetType().IsAssignableFrom(typeof(EnemyProwler))))
            {
                agent.ChangeBehavior("Find");
            }
        }
    }

    public override void Exit(Prowler agent)
    {
    }
}
