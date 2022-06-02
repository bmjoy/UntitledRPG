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
        mirror.transform.position = mOwner.transform.position;
        mirror.transform.position += new Vector3(0.0f, 0.0f, -0.4f);
        mirror.transform.SetParent(mOwner.transform);
        mirror.AddComponent<SpriteRenderer>().sprite = mOwner.GetComponent<SpriteRenderer>().sprite;
        mirror.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
        mirror.AddComponent<Billboard>();

        mirror2 = new GameObject("Mirror");
        mirror2.transform.position = mOwner.transform.position;
        mirror2.transform.position += new Vector3(0.0f, 0.0f, 0.4f);
        mirror2.transform.SetParent(mOwner.transform);
        mirror2.AddComponent<SpriteRenderer>().sprite = mOwner.GetComponent<SpriteRenderer>().sprite;
        mirror2.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
        mirror2.AddComponent<Billboard>();
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
