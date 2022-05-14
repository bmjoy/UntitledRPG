using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : Projectile
{
    [SerializeField]
    private int mCount;

    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(mChannelingTime / 2.0f);
        for (int i = 0; i < mCount; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>
                ("Prefabs/Bullets/PhantomDance_Mini"), transform.position, Quaternion.identity);
            go.GetComponent<PhantomMini>().Initialize(mTarget, () => {
                mTarget.TakeDamage(mDamage, DamageType.Magical); });
            yield return new WaitForSeconds(0.25f);
        }
    }

    protected override void Update()
    {
        base.Update();
    }
}
