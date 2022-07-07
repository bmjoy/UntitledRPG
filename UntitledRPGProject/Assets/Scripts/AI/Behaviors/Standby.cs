using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Standby : State
{
    private bool IsSucceeded = false;
    private bool IsSearched = false;
    public override void Enter(Unit agent)
    {
        agent.mConditions.isDefend = false;
        agent.mOrder = Order.Standby;
        agent.GetComponent<BuffAndNerfEntity>().Tick();
        agent.mField.GetComponent<Field>().Picked(true);
        IsSucceeded = false;
        IsSearched = false;
        while (IsSucceeded == false)
        {
            IsSucceeded = Find(agent);
        }
        if (agent.GetType() == typeof(Boss))
            agent.mAiBuild.stateMachine.ChangeState("Boss_Standby");
    }

    public override void Execute(Unit agent)
    {
        if (agent.GetType() == typeof(Boss)) return;

        if (agent.mAiBuild.type == AIType.Auto)
        {
            if(IsSucceeded && !IsSearched)
            {
                agent.StartCoroutine(WaitforSecond(agent));
                IsSearched = true;
            }
        }
    }

    public override void Exit(Unit agent)
    {
        if (agent.GetType() == typeof(Boss)) return;
        agent.mAiBuild.stateMachine.mPreferredTarget = null;
        agent.mTarget?.mSelected.SetActive(false);
    }

    public override bool Find(Unit agent)
    {
        if (agent.mAiBuild.priority == AITargetPriority.AimToHighHealth)
        {
            SeekToHighHealthCost(agent, ref agent.mTarget);
            return true;
        }

        List<GameObject> list = new List<GameObject>((agent.mFlag == Flag.Enemy) ? PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
            : GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList());
        if (list.Count == 0)
        {
            agent.mAiBuild.stateMachine.ChangeState("Waiting");
            return true;
        }
        agent.mTarget = (agent.mAiBuild.stateMachine.mPreferredTarget) ? agent.mAiBuild.stateMachine.mPreferredTarget
            : list[Random.Range(0, list.Count)].GetComponent<Unit>();
        return !agent.mTarget.mConditions.isDied;
    }

    public override void ThinkingMagic(Unit agent, ref string current)
    {
        current = ((agent.mSkillDataBase.mSkill.GetType() == typeof(SummonAbility))
|| (agent.mStatus.mMana >= agent.mSkillDataBase.Mana && UnityEngine.Random.Range(0, 100) >= 50)) ? "Magic" : current;
    }

    private void SeekToHighHealthCost(Unit agent, ref Unit target)
    {
        if (agent.mAiBuild.stateMachine.mPreferredTarget)
        {
            target = agent.mAiBuild.stateMachine.mPreferredTarget;
            return;
        }

        IEnumerable<GameObject> list = (agent.mFlag == Flag.Enemy) ? PlayerController.Instance.mHeroes
            : BattleManager.Instance.mEnemies;
        if (agent.mAiBuild.stateMachine.mPreferredTarget)
            target = agent.mAiBuild.stateMachine.mPreferredTarget;
        else
        {
            float maxHealth = 0.0f;
            float currentHealth = 0.0f;
            foreach (GameObject u in list)
            {
                currentHealth = u.GetComponent<Unit>().mStatus.mHealth;
                if (currentHealth > maxHealth)
                {
                    maxHealth = currentHealth;
                    target = u.GetComponent<Unit>();
                }
            }
        }
    }
}
