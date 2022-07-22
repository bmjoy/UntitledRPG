using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiting : State
{
    public override void Enter(Unit agent)
    {
        if(agent.gameObject.activeSelf)
            agent.StartCoroutine(WaitforSecond(agent));
    }

    public override void Execute(Unit agent)
    {
    }

    public override void Exit(Unit agent)
    {
    }

    public override bool Find(Unit agent)
    {
        return false;
    }

    public override IEnumerator WaitforSecond(Unit agent)
    {
        yield return new WaitForSeconds(2.0f);

        if (PlayerController.Instance.IsDied)
            yield break;

        for (int i = 0; i < BattleManager.Instance.mUnits.Count; i++)
        {
            Unit unit = BattleManager.Instance.mUnits[i].GetComponent<Unit>();
            unit.mField.TargetedMagicHostile(false);
            unit.mField.TargetedFriendly(false);
            unit.mField.TargetedHostile(false);
        }
    }
}
