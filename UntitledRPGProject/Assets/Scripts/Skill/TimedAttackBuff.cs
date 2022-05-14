using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedAttackBuff : TimedBuff
{
    private GameObject effectObject;
    private bool mReinforced = false;
    public TimedAttackBuff(AttackBuff buff, Unit owner, Unit target) : base(buff, owner, target)
    {
        if (mTarget.transform.Find(Buff.name + "(Clone)") == null && mTarget.mStatus.mHealth > 0.0f)
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + Buff.name), new Vector3(mTarget.transform.position.x, mTarget.transform.position.y + 0.5f, mTarget.transform.position.z + 0.2f), Quaternion.identity);
            go.transform.parent = mTarget.transform;
            effectObject = go;
        }
    }

    public override void End()
    {
        if(mReinforced)
        {
            var AttackUp = (AttackBuff)Buff;
            mTarget.mStatus.mDamage = mTarget.mStatus.mDamage / AttackUp.mMultiplier - (mOwner.mStatus.mMagicPower * AttackUp.mMagicPowerMultiplier);
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
            mTarget.mStatus.mDamage = mTarget.mStatus.mDamage * AttackUp.mMultiplier + (mOwner.mStatus.mMagicPower * AttackUp.mMagicPowerMultiplier);
            mReinforced = true;
        }
    }
}
