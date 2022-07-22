using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void Enter(Unit agent);
    public abstract void Execute(Unit agent);
    public abstract void Exit(Unit agent);
    public abstract bool Find(Unit agent);

    public virtual IEnumerator WaitforSecond(Unit agent)
    {
        float randomNumber = -1.0f;
        yield return new WaitForSeconds(0.5f);
        int percent = Mathf.RoundToInt((100 * agent.mStatus.mHealth) / agent.mStatus.mMaxHealth);
        int targetPercent = Mathf.RoundToInt((100 * agent.mTarget.mStatus.mHealth) / agent.mTarget.mStatus.mMaxHealth);
        randomNumber = (agent.mAiBuild.property == AIBuild.AIProperty.Offensive) ? Random.Range(10, 20) : Random.Range(40, 60);

        bool condition1 = percent > randomNumber;
        bool condition2 = percent > targetPercent;
        bool condition3 = agent.mBuffNerfController.GetBuffCount() > 0;

        string behavior = ((condition1 || condition2 || condition3) && agent.mType != AttackType.None) ? "Attack" : "Defend";

        agent.mTarget?.mSelected.SetActive(true);
        if (agent.mSkillDataBase != null)
            ThinkingMagic(agent, ref behavior);

        if (agent.mAiBuild.stateMachine.mPreferredTarget)
            behavior = (agent.mType != AttackType.None) ? "Attack" : "Defend";
        agent.mAiBuild.stateMachine.ChangeState(behavior);
        agent.mTarget.mField.TargetedHostile((behavior.Contains("Attack")));
    }

    public virtual void ThinkingMagic(Unit agent, ref string current) {}
}

public abstract class P_State
{
    public abstract void Enter(Prowler agent);
    public abstract void Execute(Prowler agent);
    public abstract void Exit(Prowler agent);
}
