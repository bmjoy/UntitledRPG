using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Find : P_State
{
    GameObject player;
    Vector3 dest = Vector3.zero;
    float mTime = 0.0f;
    float mFacedTime = 0.0f;
    bool isFound = false;

    public override void Enter(Prowler agent)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").gameObject;
        mTime = mFacedTime = 0.0f;
        isFound = false;
        agent.mAnimator.SetFloat("Speed", agent.mSpeed);
        NavMeshHit mNavHit;

        bool found = NavMesh.SamplePosition(agent.transform.position + new Vector3((float)Random.Range(-3, 3), 0.0f, (float)Random.Range(-3, 3)),
            out mNavHit, 3.0f, 3);
        dest = (found) ? mNavHit.position : agent.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0.0f, Random.Range(-0.5f, 0.5f)); ;
    }
    public override void Execute(Prowler agent)
    {
        mTime += Time.deltaTime;
        agent.transform.position = Vector3.MoveTowards(agent.transform.position, dest + new Vector3(0.0f,1.0f,0.0f), Time.deltaTime * agent.mSpeed);
        Vector3 dir = (player.transform.position - agent.transform.position).normalized;
        if (Vector3.Dot(dir, agent.transform.position) > Mathf.Cos(agent.mAngle) && isFound == false)
        {
            float dist = Vector3.Distance(agent.transform.position, player.transform.position);
            if (dist <= agent.mRadius)
            {
                EnemyProwler enemyProwler = (EnemyProwler)agent;
                enemyProwler.mExclamation.SetActive(true);
                enemyProwler.mParticles.SetActive(true);
                enemyProwler.mParticles.GetComponent<ParticleSystem>().Play();
                isFound = true;
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mExclamationSFX);
            }
        }

        if (mFacedTime >= 0.5f && isFound)
            agent.ChangeBehavior("Pursuit");
        if (isFound)
            mFacedTime += Time.deltaTime;
        else
        {
            if (mTime > agent.mStandbyTime || Vector3.Distance(agent.transform.position, dest) < 0.5f)
                agent.ChangeBehavior("Idle");
        }
    }

    public override void Exit(Prowler agent)
    {
        mTime = 0.0f;
        agent.mAnimator.SetFloat("Speed", 0.0f);
    }
}
