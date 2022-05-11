using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedNerf
{
    protected int mTurn;
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
            if (mTurn <= 0)
            {
                isActive = false;
                End();
            }
            else
                Apply();
            mTurn--;
        }
    }

    public void Activate()
    {
        if (!Nerf.IsTurnFinished || mTurn <= 0)
            mTurn += Nerf.mTurn;
        else if (!Nerf.IsTurnFinished)
            mTurn = Nerf.mTurn;
    }

    public void Inactivate()
    {
        isActive = false;
        End();
    }

    protected abstract void Apply();
    public abstract void End();
}
