using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public int mTurn;
    public bool IsTurnFinished = false;
    public bool IsEffectFinished = false;
    public abstract TimedBuff Initialize(Unit unit);
}
