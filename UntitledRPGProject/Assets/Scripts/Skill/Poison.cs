using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nerfs/Poison")]
public class Poison : Nerf
{
    [Range(0.0f, 1.0f)]
    public float mMagicPowerMultiplier = 0.6f;
    public float mDamage;

    public override TimedNerf Initialize(Unit owner, Unit target)
    {
        return new TimedPoison(this,owner,target);
    }
}
