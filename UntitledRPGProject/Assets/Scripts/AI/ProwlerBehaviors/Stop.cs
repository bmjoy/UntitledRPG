using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : P_State
{
    float mTime = 0.0f;
    public override void Enter(Prowler agent)
    {
    }

    public override void Execute(Prowler agent)
    {
        agent.mRigidbody.velocity = Vector3.zero;
        agent.mRigidbody.angularVelocity = Vector3.zero;
        if (LevelManager.Instance.isNext)
            return;
        mTime += Time.deltaTime;
        if (mTime > agent.mStandbyTime * 2.0f)
        {
            if ((agent.GetType().IsAssignableFrom(typeof(EnemyProwler))))
            {
                agent.ChangeBehavior("Find");
            }
        }
    }

    public override void Exit(Prowler agent)
    {
    }
}
