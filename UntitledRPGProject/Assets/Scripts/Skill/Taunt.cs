using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nerfs/Taunt")]
public class Taunt : Nerf
{
    [Range(0.0f, 1.0f)]
    public float mChanceRate = 1.0f;
    public override TimedNerf Initialize(Unit owner, Unit target)
    {
        return new TimedTaunt(this, owner, target);
    }
}
