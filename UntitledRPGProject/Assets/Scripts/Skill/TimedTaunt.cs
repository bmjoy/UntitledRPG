using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTaunt : TimedNerf
{
    private GameObject effectObject;
    private Taunt taunt;

    public TimedTaunt(Taunt nerf, Unit owner, Unit target) : base(nerf, owner, target)
    {
        taunt = (Taunt)Nerf;
        if (taunt.mChanceRate >= Random.Range(0.0f, 1.0f))
        {

            if (ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name) == null)
                return;
            var t = target.transform.Find(Nerf.name + "(Clone)");
            if (target.transform.Find(Nerf.name + "(Clone)") == null && target.mStatus.mHealth > 0)
            {
                GameObject go = Object.Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(target.transform.position.x, target.transform.position.y + target.GetComponent<BoxCollider>().size.y * 0.5f, target.transform.position.z), Quaternion.identity);
                go.transform.parent = target.transform;
                effectObject = go;
            }
            else
            {
                Object.Destroy(t.gameObject);
                GameObject go = Object.Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(target.transform.position.x, target.transform.position.y + target.GetComponent<BoxCollider>().size.y * 0.5f, target.transform.position.z), Quaternion.identity);
                go.transform.parent = target.transform;
                effectObject = go;
            }
            mTarget.mConditions.isDefend = false;
        }
    }

    public override void End()
    {
        taunt.IsTurnFinished = true;
        mTarget.mAiBuild.stateMachine.mPreferredTarget = null;
        if (mTarget.mFlag == Flag.Player)
            mTarget.mAiBuild.type = AIBuild.AIType.Manual;
        if(effectObject)
            Object.Destroy(effectObject);
        var t = mTarget.transform.Find(Nerf.name + "(Clone)");
        if(t)
        {
            Object.Destroy(t.gameObject);
        }
    }

    protected override void Apply()
    {
        mTarget.mAiBuild.stateMachine.mPreferredTarget = mOwner;
        if (mTarget.mFlag == Flag.Player)
            mTarget.mAiBuild.type = AIBuild.AIType.Auto;
        if (mOwner.mConditions.isDied || mTarget.mConditions.isDied)
            End();
    }
}
