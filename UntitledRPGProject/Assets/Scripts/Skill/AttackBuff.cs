using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/AttackBuff")]
public class AttackBuff : Buff
{
    public float mMultiplier = 2.0f;
    public override TimedBuff Initialize(Unit owner, Unit target)
    {
        return new TimedAttackBuff(this, owner, target);
    }
}
