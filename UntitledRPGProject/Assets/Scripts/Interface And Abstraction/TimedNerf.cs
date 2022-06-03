using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedNerf
{
    protected int mTurn;
    public bool isActive = false;
    public Nerf Nerf { get; }
    protected readonly Unit mOwner;
    protected readonly Unit mTarget;
    public TimedNerf(Nerf nerf, Unit owner, Unit target)
    {
        Nerf = nerf;
        mOwner = owner;
        mTarget = target;
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
            {
                if (Nerf.NerfTick)
                    AudioManager.PlaySfx(Nerf.NerfTick);
                Apply();
            }
            mTurn--;
        }
    }

    public void Activate()
    {
        if (Nerf.NerfStart)
            AudioManager.PlaySfx(Nerf.NerfStart);
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
