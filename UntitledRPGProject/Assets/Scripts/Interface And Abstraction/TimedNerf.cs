using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedNerf
{
    protected float mTurn;
    public bool isActive = false;
    public Nerf Nerf { get; }
    protected readonly Unit mOwner;

    public TimedNerf(Nerf nerf, Unit unit)
    {
        Nerf = nerf;
        mOwner = unit;
    }

    public void Tick()
    {
        if (isActive)
        {
            mTurn--;
            if (mTurn <= 0)
            {
                isActive = false;
                End();
            }
        }
    }

    public void Activate()
    {
        if(Nerf.IsEffectFinished || mTurn <= 0)
        {
            Apply();
        }

        if(Nerf.IsTurnFinished || mTurn <= 0)
        {
            mTurn += Nerf.mTurn;
        }
    }

    protected abstract void Apply();
    public abstract void End();
}
