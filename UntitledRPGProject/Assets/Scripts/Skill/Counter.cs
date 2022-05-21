using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Counter")]
public class Counter : Buff
{
    public float mMultiplier = 2.0f;

    public override TimedBuff Initialize(Unit owner, Unit target)
    {
        return new TimedCounter(this, owner, target);
    }
}
