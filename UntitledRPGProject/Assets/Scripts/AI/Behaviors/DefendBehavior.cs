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
        var skill = agent.GetComponent<Skill_DataBase>();

        if (skill == null)
        {
            isMagic = false;
            return;
        }

        if (skill.Skill == null)
        {
            isMagic = false;
            return;
        }

        if (agent.mStatus.mMana < skill.Mana)
        {
            isMagic = false;
            return;
        }
        else
        {
            list = (agent.mFlag == Flag.Enemy) ? GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
    : PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList();
            isMagic = (UnityEngine.Random.Range(0, 100) >= 50) ? true : false;
        }
    }

    public override void Execute(Unit agent)
    {
        if (isMagic)
        {
            var skill = agent.GetComponent<Skill_DataBase>().Skill;
            if (skill.mProperty == SkillProperty.Friendly)
            {
                switch (skill.mSkillType)
                {
                    case SkillType.Heal:
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
            else
                BattleManager.Instance.Defend();
        }
        else
            BattleManager.Instance.Defend();

        agent.mAiBuild.stateMachine.ChangeState("Waiting");
    }

    public override void Exit(Unit agent)
    {
        agent.mConditions.isPicked = false;
    }
}
