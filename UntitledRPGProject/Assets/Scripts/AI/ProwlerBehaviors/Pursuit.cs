using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : P_State
{
    float mTime = 0.0f;
    float mMaximumStandbyTime = 4.0f;
    public override void Enter(EnemyProwler agent)
    {
        mTime = 0.0f;
        agent.mAgent.speed = 5.0f;
        agent.mAgent.SetDestination(agent.mLastPos);
        agent.mAnimator.SetFloat("Speed", agent.mAgent.speed);
    }

    public override void Execute(EnemyProwler agent)
    {
        mTime += Time.deltaTime;
        agent.mVelocity = agent.mAgent.velocity;
        if (mTime > mMaximumStandbyTime)
            agent.ChangeBehavior("Find");
    }

    public override void Exit(EnemyProwler agent)
    {
        agent.mAgent.speed = agent.mOriginalSpeed;
        agent.mAnimator.SetFloat("Speed", 0.0f);
    }
}
