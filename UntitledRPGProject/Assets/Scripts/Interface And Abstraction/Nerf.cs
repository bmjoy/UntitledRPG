using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Nerf : ScriptableObject
{
    public GameObject mEffect;
    public int mTurn;
    public bool IsTurnFinished = false;
    public abstract TimedNerf Initialize(Unit unit);
}
