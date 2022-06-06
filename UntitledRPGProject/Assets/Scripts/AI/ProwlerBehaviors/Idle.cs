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
        agent.mAgent.velocity = Vector3.zero;
        if(agent.gameObject.activeSelf)
            agent.mAgent.isStopped = true;
    }

    public override void Execute(Prowler agent)
    {
        agent.mAgent.velocity = Vector3.zero;
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
