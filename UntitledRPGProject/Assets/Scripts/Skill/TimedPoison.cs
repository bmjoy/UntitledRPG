using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoison : TimedNerf
{
    private GameObject effectObject;
    private Poison poison;

    public TimedPoison(Poison nerf, Unit owner, Unit target) : base(nerf, owner, target)
    {
        if (Resources.Load<GameObject>("Prefabs/Effects/" + Nerf.name) == null)
            return;
        poison = (Poison)Nerf;
        poison.IsTurnFinished = false;
        if (target.transform.Find(Nerf.name + "(Clone)") == null && target.mStatus.mHealth > ((mOwner.mStatus.mMagicPower * poison.mMagicPowerMultiplier)) - target.mMagicDistance)
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(target.transform.position.x, target.transform.position.y + target.GetComponent<BoxCollider>().size.y * 0.5f, target.transform.position.z), Quaternion.identity);
            go.transform.parent = target.transform;
            effectObject = go;
        }
    }

    public override void End()
    {
        poison.IsTurnFinished = true;
        GameObject.Destroy(effectObject);
    }

    protected override void Apply()
    {
        poison = (Poison)Nerf;
        mTarget.TakeDamage(poison.mDamage + mOwner.mStatus.mMagicPower * poison.mMagicPowerMultiplier, DamageType.Magical);
        if (mOwner.mConditions.isDied || mTarget.mConditions.isDied)
            End();
    }
}
