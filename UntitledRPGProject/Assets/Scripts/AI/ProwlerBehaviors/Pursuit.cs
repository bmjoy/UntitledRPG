using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : P_State
{
    public override void Enter(Prowler agent)
    {
        agent.mSpeed = agent.mSpeed * 1.5f;
        agent.mAnimator.SetFloat("Speed", agent.mSpeed);
    }

    public override void Execute(Prowler agent)
    {
        Vector3 direction = (PlayerController.Instance.transform.position - agent.transform.position).normalized;
        float dist = Vector3.Distance(agent.transform.position, PlayerController.Instance.transform.position);
        bool check = !Physics.Raycast(agent.transform.position, direction, dist, LayerMask.GetMask("Obstacle"));
        if(!check || dist > agent.mRadius + agent.mRadius)
            agent.ChangeBehavior("Idle");
        else
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, PlayerController.Instance.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Time.deltaTime * agent.mSpeed);
        if (LevelManager.Instance.isNext)
        {
            agent.mRigidbody.velocity = Vector3.zero;
            agent.mRigidbody.angularVelocity = Vector3.zero;
            agent.ChangeBehavior("Stop");
        }
    }
    public override void Exit(Prowler agent)
    {
        agent.mSpeed = agent.mOriginalSpeed;
        agent.mAnimator.SetFloat("Speed", 0.0f);
        EnemyProwler enemyProwler = (EnemyProwler)agent;
        enemyProwler.mExclamation.SetActive(false);
        enemyProwler.mParticles.SetActive(false);
    }
}
