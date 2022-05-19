using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackBehavior : State
{
    private bool isMagic = true;
    public override void Enter(Unit agent)
    {
        Skill_DataBase skill = agent.GetComponent<Skill_DataBase>();
        if (skill == null)
        {
            isMagic = false;
            return;
        }

        if(skill.Skill == null)
        {
            isMagic = false;
            return;
        }

        if(agent.mStatus.mMana < skill.Mana)
        {
            isMagic = false;
            return;
        }
        else
        {
            int percentage = UnityEngine.Random.Range(0, 100);
            isMagic = (percentage >= 50) ? true : false;
        }
    }

    public override void Execute(Unit agent)
    {
        if (isMagic)
            BattleManager.Instance.Magic();
        else
            BattleManager.Instance.Attack();
        agent.mAiBuild.stateMachine.ChangeState("Waiting");
    }

    public override void Exit(Unit agent)
    {
        agent.mConditions.isPicked = false;
    }
}
