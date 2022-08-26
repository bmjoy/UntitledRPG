using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nerfs/Stun")]
public class Stun : Nerf
{
    [Range(0.0f, 1.0f)]
    public float mChanceRate = 0.2f;
    public override TimedNerf Initialize(Unit owner, Unit target)
    {
        return new TimedStun(this, owner, target);
    }
}
