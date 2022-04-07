using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefendBehavior : State
{
    private List<GameObject> list = new List<GameObject>();
    private bool isMagic = true;
    public override void Enter(Unit agent)
    {
        if (agent.mSkillDataBase == null)
        {
            isMagic = false;
            return;
        }

        if (agent.mSkillDataBase.mSkill == null)
        {
            isMagic = false;
            return;
        }

        if (agent.mStatus.mMana < agent.mSkillDataBase.mSkill.mManaCost)
        {
            isMagic = false;
            return;
        }
        else
        {
            list = (agent.mFlag == Flag.Enemy) ? GameManager.Instance.mEnemyProwler.EnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
    : GameManager.Instance.mPlayer.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList();
            int percentage = UnityEngine.Random.Range(0, 100);
            isMagic = (percentage >= 50) ? true : false;
        }
    }

    public override void Execute(Unit agent)
    {
        if (isAct)
        {
            if (agent.mOrder == Order.TurnEnd)
                agent.mAiBuild.stateMachine.ChangeState("Standby");
            return;
        }
        if (isMagic)
        {
            var skill = agent.mSkillDataBase.mSkill;
            if (skill.mProperty == SkillProperty.Friendly)
            {
                switch (skill.mSkillType)
                {
                    case SkillType.Heal:
                    case SkillType.HealBuff:
                        {
                            float minPercent = float.MaxValue;
                            for (int i = 0; i < list.Count; i++)
                            {
                                var unit = list[i].GetComponent<Unit>();
                                float percent = Mathf.Round((100.0f * unit.mStatus.mHealth) / unit.mStatus.mMaxHealth);
                                if (minPercent > percent)
                                {
                                    minPercent = percent;
                                    agent.mTarget = unit;
                                }
                            }
                        }
                        break;
                    case SkillType.BuffNerf:
                    case SkillType.Buff:
                        {
                            float maxDamage = float.MaxValue;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (maxDamage > agent.mStatus.mDamage)
                                {
                                    maxDamage = agent.mStatus.mDamage;
                                    agent.mTarget = list[i].GetComponent<Unit>();
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

            }
        }
        else
            BattleManager.Instance.Defend();
        isAct = true;
    }

    public override void Exit(Unit agent)
    {
        isAct = false;
    }
}
