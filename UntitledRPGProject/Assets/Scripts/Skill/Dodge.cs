using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Dodge")]
public class Dodge : Buff
{
    public float mChanceRate = 0.15f;
    public override TimedBuff Initialize(Unit owner, Unit target)
    {
        return new TimedDodge(this, owner, target);
    }
}
