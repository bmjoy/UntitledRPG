using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefendBehavior : State
{
    public override void Enter(Unit agent)
    {
    }

    public override void Execute(Unit agent)
    {
        BattleManager.Instance.Defend();
        agent.mAiBuild.stateMachine.ChangeState("Waiting");
    }

    public override void Exit(Unit agent)
    {
        agent.mAiBuild.stateMachine.mPreferredTarget = null;
        agent.mTarget = null;
        if (PlayerController.Instance.IsDied || BattleManager.Instance.mUnits.Count == 0)
            return;
        for (int i = 0; i < BattleManager.Instance.mUnits.Count; i++)
        {
            Unit unit = BattleManager.Instance.mUnits[i].GetComponent<Unit>();
            unit.mField.TargetedMagicHostile(false);
            unit.mField.TargetedFriendly(false);
            unit.mField.TargetedHostile(false);
        }
    }

    public override bool Find(Unit agent)
    {
        return false;
    }
}
