using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackBehavior : State
{
    public override void Enter(Unit agent)
    {
    }

    public override void Execute(Unit agent)
    {
        BattleManager.Instance.Attack();
        agent.mAiBuild.stateMachine.ChangeState("Waiting");
    }

    public override void Exit(Unit agent)
    {
    }
}
