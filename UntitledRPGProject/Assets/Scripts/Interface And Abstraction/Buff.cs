using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public int mTurn;
    public bool IsTurnFinished = false;
    public float mMagicPowerMultiplier = 0.6f;
    public abstract TimedBuff Initialize(Unit owner, Unit target);
}
