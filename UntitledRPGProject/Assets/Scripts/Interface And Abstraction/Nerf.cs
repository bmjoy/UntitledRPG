using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Nerf : ScriptableObject
{
    public int mTurn;
    public bool IsTurnFinished = false;
    public float mMagicPowerMultiplier = 0.6f;
    public abstract TimedNerf Initialize(Unit owner, Unit target);
}
