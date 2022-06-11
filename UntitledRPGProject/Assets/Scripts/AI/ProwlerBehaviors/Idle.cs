using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : P_State
{
    float mTime = 0.0f;
    public override void Enter(Prowler agent)
    {
        mTime = 0.0f;
    }

    public override void Execute(Prowler agent)
    {
        if (LevelManager.Instance.isNext)
            return;
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
