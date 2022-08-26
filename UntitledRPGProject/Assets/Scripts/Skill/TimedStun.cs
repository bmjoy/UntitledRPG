using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStun : TimedNerf
{
    private GameObject effectObject;
    private Stun stun;

    public TimedStun(Stun nerf, Unit owner, Unit target) : base(nerf, owner, target)
    {
    }

    public override void End()
    {
        stun.IsTurnFinished = true;
        if (mTarget.mFlag == Flag.Player)
            mTarget.mAiBuild.type = AIBuild.AIType.Manual;
        if (effectObject)
            GameObject.Destroy(effectObject);
        var t = mTarget.transform.Find(Nerf.name + "(Clone)");
        if(t)
        {
            Object.Destroy(t.gameObject);
        }
    }

    protected override void Apply()
    {
        stun = (Stun)Nerf;
        if (stun.mChanceRate >= Random.Range(0.0f, 1.0f))
        {
            if (mTarget.mFlag == Flag.Player)
                mTarget.mAiBuild.type = AIBuild.AIType.Auto;
            if (ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name) == null)
                return;

            var t = mTarget.transform.Find(Nerf.name + "(Clone)");
            if (t == null && mTarget.mStatus.mHealth > 0)
            {
                GameObject go = Object.Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(mTarget.transform.position.x, mTarget.transform.position.y + mTarget.GetComponent<BoxCollider>().size.y * 0.5f, mTarget.transform.position.z), Quaternion.identity);
                go.transform.parent = mTarget.transform;
                effectObject = go;
            }
            else
            {
                Object.Destroy(t.gameObject);
                GameObject go = Object.Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(mTarget.transform.position.x, mTarget.transform.position.y + mTarget.GetComponent<BoxCollider>().size.y * 0.5f, mTarget.transform.position.z), Quaternion.identity);
                go.transform.parent = mTarget.transform;
                effectObject = go;
            }
        }
        else
        {
            mTurn--;
        }

        if (mOwner.mConditions.isDied || mTarget.mConditions.isDied)
            End();
    }
}
