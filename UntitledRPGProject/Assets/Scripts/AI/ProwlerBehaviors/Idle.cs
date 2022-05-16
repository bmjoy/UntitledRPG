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
        agent.mAnimator.SetFloat("Speed", 0.0f);
    }

    public override void Execute(Prowler agent)
    {
        mTime += Time.deltaTime;
        if (mTime > agent.mStandbyTime)
            agent.ChangeBehavior((agent.GetType().Name == "EnemyProwler") ? "Find" : "Wander");
    }

    public override void Exit(Prowler agent)
    {
    }
}
