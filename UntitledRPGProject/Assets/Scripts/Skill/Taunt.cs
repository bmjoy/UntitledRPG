using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Nerfs/Taunt")]
public class Taunt : Nerf
{
    public override TimedNerf Initialize(Unit owner, Unit target)
    {
        return new TimedTaunt(this, owner, target);
    }
}
