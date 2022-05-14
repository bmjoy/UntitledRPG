using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nerfs/Poison")]
public class Poison : Nerf
{
    public float mDamage;

    public override TimedNerf Initialize(Unit owner, Unit target)
    {
        return new TimedPoison(this,owner,target);
    }
}
