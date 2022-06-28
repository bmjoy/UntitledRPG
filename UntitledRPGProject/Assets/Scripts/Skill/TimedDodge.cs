using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDodge : TimedBuff
{
    private bool mReinforced = false;
    GameObject mirror;
    GameObject mirror2;
    public TimedDodge(Buff buff, Unit owner, Unit target) : base(buff, owner, target)
    {  
        mirror = new GameObject("Mirror");
        mirror.AddComponent<Mirror>().Initialize(mOwner, new Vector3(0.0f, 0.0f, 0.6f));

        mirror2 = new GameObject("Mirror");
        mirror2.AddComponent<Mirror>().Initialize(mOwner, new Vector3(0.0f, 0.0f, -0.6f));
        mirror.GetComponent<Mirror>().Link(mirror2.GetComponent<Mirror>());
    }

    public override void End()
    {
        if(mReinforced)
        {
            Buff.IsTurnFinished = true;
            mReinforced = false;
        }
        Object.Destroy(mirror);
        Object.Destroy(mirror2);
    }

    protected override void Apply()
    {
        if(!mReinforced)
        {
            mReinforced = true;
        }
    }
}
