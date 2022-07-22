using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Standby : State
{
    private bool IsSearched = false;
    public override void Enter(Unit agent)
    {
        IsSearched = false;
    }

    public override void Execute(Unit agent)
    {
        if (agent.mAiBuild.type == AIBuild.AIType.Auto)
        {
            if (!IsSearched)
            {
                agent.StartCoroutine(WaitforSecond(agent));
                IsSearched = true;
            }
        }
    }

    public override void Exit(Unit agent)
    {
        agent.mAiBuild.stateMachine.mPreferredTarget = null;
        agent.mTarget?.mSelected.SetActive(false);
    }

    public override bool Find(Unit agent)
    {
        return false;
    }

    public override void ThinkingMagic(Unit agent, ref string current)
    {
        Boss boss = (Boss)agent;
        var database = boss.GetComponent<Boss_Skill_DataBase>();
        if (boss.mHealthTriggerPercentage.Length > 1)
        {
            if (boss.mActionTriggerComponent._isUltimate)
            {
                database.ChangeSkill(database.mUltimateSkillIndex);
                current = "Magic";
                if (database.mSkill.GetType().IsAssignableFrom(typeof(SelfAbility)))
                {
                    for (int i = 0; i < PlayerController.Instance.mHeroes.Count; i++)
                    {
                        Unit unit = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                        if (!unit.mConditions.isDied)
                        {
                            unit.mField.TargetedMagicHostile(true);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < boss.mHealthTriggerPercentage.Length; ++i)
                {
                    if (boss.HalfHealthEvent(boss.mHealthTriggerPercentage[i]))
                    {
                        database.ChangeSkill(i);
                        current = (database.Mana <= boss.mStatus.mMana) ? "Magic" : "Attack";
                    }
                }
                if (agent.mBuffNerfController.GetBuffCount() > 0)
                    current = "Attack";
                boss.mTarget.mField.TargetedMagicHostile(boss.mSkillDataBase.mSkill.mProperty == SkillProperty.Hostile);
                boss.mTarget.mField.TargetedMagicHostile(boss.mSkillDataBase.mSkill.mProperty == SkillProperty.Friendly);
            }
        }
        else
        {
            if (boss.HalfHealthEvent(boss.mHealthTriggerPercentage[0]))
            {
                if (database.Type == SkillType.Buff)
                {
                    if (boss.mBuffNerfController.GetBuffCount() > 0)
                        current = "Attack";
                    else
                        current = (database.Mana <= boss.mStatus.mMana) ? "Magic" : "Attack";
                }
                else
                {
                    current = (database.Mana <= boss.mStatus.mMana) ? "Magic" : "Attack";
                }
            }
        }

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
                    if (agent.mFlag == Flag.Player)
                    {
                        for (int i = 0; i < BattleManager.Instance.mEnemies.Count; ++i)
                        {
                            var unit = BattleManager.Instance.mEnemies[i].GetComponent<Unit>();
                            unit.mField.TargetedMagicHostile(true);
                        }
                    }
                    else if (agent.mFlag == Flag.Enemy)
                    {
                        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
                        {
                            var unit = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                            unit.mField.TargetedMagicHostile(true);
                        }
                    }
                    else
                    {

                    }
                }
                else if (ability.mSkillNerfTarget == SkillTarget.Self)
                {
                    agent.mField.TargetedMagicHostile(true);
                }
                else
                {
                    agent.mField.TargetedMagicHostile(true);
                }
                if (ability.mSkillBuffTarget == SkillTarget.All)
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
                    else
                    {

                    }
                }
                else if (ability.mSkillBuffTarget == SkillTarget.Self)
                {
                    agent.mField.TargetedFriendly(true);
                }
                else
                {

                }

            }
            else
            {
                TargetAbility ability = (TargetAbility)agent.mSkillDataBase.mSkill;
                if (ability.mProperty == SkillProperty.Friendly)
                {
                    agent.mTarget.mField.TargetedFriendly(true);
                }
                else
                {
                    agent.mTarget.mField.TargetedMagicHostile(true);
                }
            }
        }


    }
}
