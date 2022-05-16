using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Find : P_State
{
    GameObject player;
    Vector3 dest = Vector3.zero;
    float mTime = 0.0f;
    public override void Enter(Prowler agent)
    {
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
        mTime = 0.0f;
        agent.mAnimator.SetFloat("Speed", agent.mAgent.speed);
        NavMeshHit mNavHit;
        NavMesh.SamplePosition(agent.transform.position + new Vector3((float)Random.Range(-3, 3), 0.0f, (float)Random.Range(-3, 3)),
            out mNavHit, 3.0f, 3);
        agent.mAgent.SetDestination(mNavHit.position);
        dest = agent.mAgent.destination;
    }

    public override void Execute(Prowler agent)
    {
        mTime += Time.deltaTime;
        agent.mVelocity = agent.mAgent.velocity;
        Vector3 dir = (player.transform.position - agent.transform.position).normalized;
        if (Vector3.Dot(dir, agent.transform.position) > Mathf.Cos(agent.mAngle))
        {
            float dist = Vector3.Distance(agent.transform.position, player.transform.position);
            if (dist <= agent.mRadius)
            {
                agent.mLastPos = player.transform.position;
                agent.ChangeBehavior("Pursuit");
            }
        }


        if(mTime > agent.mStandbyTime || Vector3.Distance(agent.transform.position, dest) < 1.1f)
            agent.ChangeBehavior("Idle");
    }

    public override void Exit(Prowler agent)
    {
        mTime = 0.0f;
        agent.mAnimator.SetFloat("Speed", 0.0f);
    }
}
