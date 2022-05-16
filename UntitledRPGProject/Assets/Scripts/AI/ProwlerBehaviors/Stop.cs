using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stop : P_State
{
    float mTime = 0.0f;
    GameObject player;
    public override void Enter(Prowler agent)
    {
        agent.mAnimator.SetFloat("Speed", 0.0f);
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    public override void Execute(Prowler agent)
    {
        mTime += Time.deltaTime;
        if (mTime > agent.mStandbyTime * 2.0f || 
            (Vector3.Distance(player.transform.position, agent.transform.position) > 4.0f))
            agent.ChangeBehavior("Idle");

    }

    public override void Exit(Prowler agent)
    {
    }
}
