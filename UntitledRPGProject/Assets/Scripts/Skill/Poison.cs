using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nerfs/Poison")]
public class Poison : Nerf
{
    public float mDamage;

    public override TimedNerf Initialize(Unit unit)
    {
        return new TimedPoison(this, unit);
    }
}
