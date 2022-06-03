using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : P_State
{
    public override void Enter(Prowler agent)
    {
        agent.mAgent.speed = agent.mSpeed * 3.0f;
        agent.mAnimator.SetFloat("Speed", agent.mAgent.speed);
    }

    public override void Execute(Prowler agent)
    {
        agent.mAgent.SetDestination(PlayerController.Instance.transform.position);
        float dist = Vector3.Distance(agent.transform.position, PlayerController.Instance.transform.position);
        if (dist > agent.mRadius + agent.mRadius)
            agent.ChangeBehavior("Find");
    }

    public override void Exit(Prowler agent)
    {
        agent.mAgent.speed = agent.mOriginalSpeed;
        agent.mAnimator.SetFloat("Speed", 0.0f);
        EnemyProwler enemyProwler = (EnemyProwler)agent;
        enemyProwler.mExclamation.SetActive(false);
        enemyProwler.mParticles.SetActive(false);
    }
}
