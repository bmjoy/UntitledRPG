using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedAttackBuff : TimedBuff
{
    private GameObject effectObject;
    private bool mReinforced = false;
    public TimedAttackBuff(AttackBuff buff, Unit owner, Unit target) : base(buff, owner, target)
    {
        if (ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Buff.name) == null)
            return;
        if (mTarget.transform.Find(Buff.name + "(Clone)") == null && mTarget.mStatus.mHealth > 0.0f)
        {
            GameObject go = Object.Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Buff.name), new Vector3(mTarget.transform.position.x, mTarget.transform.position.y + 0.5f, mTarget.transform.position.z),
                new Quaternion(0.0f,0.0f,0.0f,1.0f));
            go.GetComponent<Renderer>().sortingLayerName = "Foreground";
            go.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
            go.transform.parent = mTarget.transform;
            effectObject = go;
        }
    }

    public override void End()
    {
        if(mReinforced)
        {
            var AttackUp = (AttackBuff)Buff;
            mTarget.mStatus.mDamage = mTarget.mStatus.mDamage / AttackUp.mMultiplier;
            Buff.IsTurnFinished = true;
            mReinforced = false;
        }
        GameObject.Destroy(effectObject);
    }

    protected override void Apply()
    {
        if(!mReinforced)
        {
            var AttackUp = (AttackBuff)Buff;
            mTarget.mStatus.mDamage = mTarget.mStatus.mDamage * AttackUp.mMultiplier;
            mReinforced = true;
        }
    }
}
