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
        if (target.transform.Find(Nerf.name + "(Clone)") == null && target.mStatus.mHealth > 0)
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(target.transform.position.x, target.transform.position.y + target.GetComponent<BoxCollider>().size.y * 0.5f, target.transform.position.z), Quaternion.identity);
            go.transform.parent = target.transform;
            effectObject = go;
        }
    }

    public override void End()
    {
        taunt.IsTurnFinished = true;
        mTarget.mAiBuild.stateMachine.mPreferredTarget = null;
        GameObject.Destroy(effectObject);
    }

    protected override void Apply()
    {
        taunt = (Taunt)Nerf;
        mTarget.mAiBuild.stateMachine.mPreferredTarget = mOwner;
        if (mOwner.mConditions.isDied || mTarget.mConditions.isDied)
            End();
    }
}
