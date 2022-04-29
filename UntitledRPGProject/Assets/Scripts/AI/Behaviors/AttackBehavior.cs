using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackBehavior : State
{
    private bool isMagic = true;
    public override void Enter(Unit agent)
    {
        if (agent.mSkillDataBase == null)
        {
            isMagic = false;
            return;
        }

        if(agent.mSkillDataBase.mSkill == null)
        {
            isMagic = false;
            return;
        }

        if(agent.mStatus.mMana < agent.mSkillDataBase.mSkill.mManaCost)
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
        if (isAct)
        {
            if (agent.mOrder == Order.TurnEnd)
                agent.mAiBuild.stateMachine.ChangeState("Standby");
            return;
        }
        if(isMagic)
            BattleManager.Instance.Magic();
        else
            BattleManager.Instance.Attack();
        isAct = true;
    }

    public override void Exit(Unit agent)
    {
        isAct = false;
    }
}
