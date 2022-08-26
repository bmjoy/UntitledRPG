using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Standby : State
{
    private bool IsSucceeded = false;
    private bool IsSearched = false;
    public override void Enter(Unit agent)
    {
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

        if (agent.mAiBuild.type == AIBuild.AIType.Auto)
        {
            if(IsSucceeded && !IsSearched)
            {
                if (agent.mBuffNerfController.GetNerf(typeof(Stun)))
                {
                    agent.mAiBuild.stateMachine.ChangeState("Waiting");
                    agent.StartCoroutine(ProcessingAfterStunning(agent));
                }
                else
                {
                    agent.StartCoroutine(WaitforSecond(agent));
                }
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

    private IEnumerator ProcessingAfterStunning(Unit agent)
    {
        UIManager.Instance.ChangeOrderBarText($"<color=red>{agent.mSetting.Name} has been stunned!</color>");
        yield return new WaitForSeconds(1.5f);
        BattleManager.Instance.EndAction();
        agent.TurnEnded();
    }

    public override bool Find(Unit agent)
    {
        if (agent.mAiBuild.priority == AIBuild.AITargetPriority.AimToHighHealth)
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
        if(current.Contains("Magic"))
        {
            if ((agent.mSkillDataBase.mSkill.GetType() == typeof(SummonAbility)))
            {
                agent.mTarget = agent;
            }
            else if ((agent.mSkillDataBase.mSkill.GetType() == typeof(SelfAbility)))
            {
                SelfAbility ability = (SelfAbility)agent.mSkillDataBase.mSkill;
                if (ability.mSkillNerfTarget == SkillTarget.All)
                {
                    if(agent.mFlag == Flag.Player)
                    {
                        for (int i = 0; i < BattleManager.Instance.mEnemies.Count; ++i)
                        {
                            var unit = BattleManager.Instance.mEnemies[i].GetComponent<Unit>();
                            unit.mField.TargetedMagicHostile(true);
                        }
                    }
                    else if(agent.mFlag == Flag.Enemy)
                    {
                        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
                        {
                            var unit = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                            unit.mField.TargetedMagicHostile(true);
                        }
                    }
                    else {}
                }
                else
                    agent.mField.TargetedMagicHostile(true);
                if(ability.mSkillBuffTarget == SkillTarget.All)
                {
                    if (agent.mFlag == Flag.Enemy)
                    {
                        for (int i = 0; i < BattleManager.Instance.mEnemies.Count; ++i)
                        {
                            var unit = BattleManager.Instance.mEnemies[i].GetComponent<Unit>();
                            unit.mField.TargetedFriendly(true);
                        }
                    }
                    else if (agent.mFlag == Flag.Player)
                    {
                        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
                        {
                            var unit = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                            unit.mField.TargetedFriendly(true);
                        }
                    }
                    else{}
                }
                else if (ability.mSkillBuffTarget == SkillTarget.Self)
                    agent.mField.TargetedFriendly(true);
                else{}
            }
            else
            {
                TargetAbility ability = (TargetAbility)agent.mSkillDataBase.mSkill;
                if (ability.mProperty == SkillProperty.Friendly)
                    agent.mTarget.mField.TargetedFriendly(true);
                else
                    agent.mTarget.mField.TargetedMagicHostile(true);
            }
        }
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
