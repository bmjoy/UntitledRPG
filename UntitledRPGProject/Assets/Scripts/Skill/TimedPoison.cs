using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoison : TimedNerf
{
    private GameObject effectObject;
    private Poison poison;

    public TimedPoison(Poison nerf, Unit owner, Unit target) : base(nerf, owner, target)
    {
        GameObject obj = ResourceManager.GetResource<GameObject>("Prefabs/Effects/" + Nerf.name);
        if (obj == null)
            return;
        poison = (Poison)Nerf;
        poison.IsTurnFinished = false;
        if (target.transform.Find(Nerf.name + "(Clone)") == null)
        {
            GameObject go = Object.Instantiate(obj, new Vector3(target.transform.position.x, target.transform.position.y + target.GetComponent<BoxCollider>().size.y * 0.5f, target.transform.position.z), Quaternion.Euler(obj.transform.eulerAngles));
            go.transform.parent = target.transform;
            effectObject = go;
        }
        if (target.mConditions.isDied)
            End();
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
        if (mTarget.mConditions.isDied)
            End();
    }
}
