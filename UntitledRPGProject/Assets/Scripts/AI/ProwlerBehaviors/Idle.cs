using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : P_State
{
    float mTime = 0.0f;
    public override void Enter(Prowler agent)
    {
        mTime = 0.0f;
        agent.mRigidbody.velocity = Vector3.zero;
        agent.mRigidbody.angularVelocity = Vector3.zero;
    }

    public override void Execute(Prowler agent)
    {
        agent.mRigidbody.velocity = Vector3.zero;
        agent.mRigidbody.angularVelocity = Vector3.zero;
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
