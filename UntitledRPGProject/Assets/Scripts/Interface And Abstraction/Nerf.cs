using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Nerf
{
    public int mTurn;
    public bool IsTurnFinished = false;
    public bool IsEffectFinished = false;
    public abstract TimedNerf Initialize();
}
