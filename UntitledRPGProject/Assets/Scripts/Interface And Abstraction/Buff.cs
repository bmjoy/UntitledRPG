using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public int mTurn;
    public bool IsTurnFinished = false;
    public AudioClip BuffStart;
    public AudioClip BuffTick;
    public abstract TimedBuff Initialize(Unit owner, Unit target);
}
