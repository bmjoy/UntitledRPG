using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : P_State
{
    GameObject player;
    Vector3 dest = Vector3.zero;
    float mTime = 0.0f;

    public override void Enter(Prowler agent)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
        mTime = 0.0f;
        agent.mAnimator.SetFloat("Speed", agent.mSpeed);

        if (Vector3.Distance(agent.transform.position, agent.mLastPos) > 5.0f)
        {
            dest = agent.mLastPos;
        }
        else
        {
            NavMeshHit mNavHit;
            NavMesh.SamplePosition(agent.transform.position + new Vector3((float)Random.Range(-3, 3), 0.0f, (float)Random.Range(-3, 3)),
                out mNavHit, 3.0f, 3);
            dest = mNavHit.position;
        }
    }

    public override void Execute(Prowler agent)
    {
        mTime += Time.deltaTime;
        agent.mVelocity = agent.mRigidbody.velocity;
        Vector3 dir = (player.transform.position - agent.transform.position).normalized;
        if (Vector3.Dot(dir, agent.transform.position) > Mathf.Cos(agent.mAngle))
        {
            float dist = Vector3.Distance(agent.transform.position, player.transform.position);
            if (Physics.Raycast(agent.transform.position, dir, dist, -1))
            {
                agent.ChangeBehavior("Stop");
            }
        }


        if (mTime > agent.mStandbyTime || Vector3.Distance(agent.transform.position, dest) < 0.5f)
            agent.ChangeBehavior("Idle");
    }

    public override void Exit(Prowler agent)
    {
        dest = Vector3.zero;
        agent.mAnimator.SetFloat("Speed", 0.0f);
    }
}
