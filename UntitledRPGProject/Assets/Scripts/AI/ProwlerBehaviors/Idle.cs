using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : P_State
{
    GameObject player;
    float mTime = 0.0f;
    float mMaximumStandbyTime = 1.0f;
    public override void Enter(EnemyProwler agent)
    {
        mTime = 0.0f;
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
    }

    public override void Execute(EnemyProwler agent)
    {
        mTime += Time.deltaTime;
        if (mTime > mMaximumStandbyTime)
            agent.ChangeBehavior("Find");

        Vector3 dir = (player.transform.position - agent.transform.position).normalized;
        if (Vector3.Dot(dir, agent.transform.position) > Mathf.Cos(agent.mAngle))
        {
            float dist = Vector3.Distance(agent.transform.position, player.transform.position);
            if (Physics.Raycast(agent.transform.position, dir, dist, 8))
            {
                agent.mLastPos = player.transform.position;
                agent.ChangeBehavior("Pursuit");
            }
        }
    }

    public override void Exit(EnemyProwler agent)
    {
    }
}
