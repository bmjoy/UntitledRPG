using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedPoison : TimedNerf
{
    private readonly Unit target;
    private GameObject effectObject;
    private Poison poison;

    public TimedPoison(Poison nerf, Unit unit) : base(nerf, unit)
    {
        target = unit;
        poison = (Poison)Nerf;
        if (target.transform.Find(Nerf.name + "(Clone)") == null && target.mStatus.mHealth > nerf.mDamage)
        {
            GameObject go = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + Nerf.name), new Vector3(unit.transform.position.x, unit.transform.position.y + 0.5f, unit.transform.position.z), Quaternion.identity);
            go.transform.parent = unit.transform;
            effectObject = go;
        }

    }

    public override void End()
    {
        poison.IsTurnFinished = true;
        Object.Destroy(effectObject);
    }

    protected override void Apply()
    {
        target.TakeDamage(poison.mDamage, DamageType.Magical);
        if (target.mConditions.isDied)
            End();
    }
}
