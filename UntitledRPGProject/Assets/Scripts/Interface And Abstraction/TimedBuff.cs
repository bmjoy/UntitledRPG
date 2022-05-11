using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedBuff
{
    protected int mTurn;
    protected int mStack;
    public bool isActive = false;
    public Buff Buff { get; }
    protected readonly Unit mOwner;

    public TimedBuff(Buff buff, Unit unit)
    {
        Buff = buff;
        mOwner = unit;
    }

    public void Tick()
    {
        if (isActive)
        {
            mTurn--;
            if (mTurn<= 0)
                isActive = false;
        }
    }
    public void Activate()
    {
        if (Buff.IsEffectFinished || mTurn <= 0)
        {
            Apply();
            mStack++;
        }

        if (Buff.IsTurnFinished || mTurn <= 0)
        {
            mTurn += Buff.mTurn;
        }
    }

    protected abstract void Apply();
    public abstract void End();
}
