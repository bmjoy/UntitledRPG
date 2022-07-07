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
        if (agent.mAiBuild.type == AIType.Auto)
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
    }
}
