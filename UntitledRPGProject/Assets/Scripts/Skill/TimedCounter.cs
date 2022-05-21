using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCounter : TimedBuff
{
    private GameObject effectObject;
    private bool mReinforced = false;
    public TimedCounter(Buff buff, Unit owner, Unit target) : base(buff, owner, target)
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
        if (mReinforced)
        {
            var ArmorUp = (Counter)Buff;
            mTarget.mStatus.mArmor = mTarget.mStatus.mArmor / ArmorUp.mMultiplier - (mOwner.mStatus.mMagicPower * ArmorUp.mMagicPowerMultiplier);
            Find(false);
            Buff.IsTurnFinished = true;
            mReinforced = false;
        }
        GameObject.Destroy(effectObject);
    }

    protected override void Apply()
    {
        if (!mReinforced)
        {
            var ArmorUp = (Counter)Buff;
            mReinforced = true;
            mTarget.mStatus.mArmor = mTarget.mStatus.mArmor * ArmorUp.mMultiplier + (mOwner.mStatus.mMagicPower * ArmorUp.mMagicPowerMultiplier);
            Find(true);
        }
    }

    private void Find(bool active)
    {
        string name = string.Empty;
        for (int i = 0; i < mOwner.GetComponent<Animator>().parameters.Length; i++)
        {
            if (mOwner.GetComponent<Animator>().parameters[i].name == "Counter")
                name = "Counter";
        }
        if (name != string.Empty)
            mOwner.GetComponent<Animator>().SetBool(name, active);
    }
}
