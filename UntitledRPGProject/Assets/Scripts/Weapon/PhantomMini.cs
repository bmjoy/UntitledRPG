using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhantomMini : Phantom
{
    private Rigidbody mRigidbody;

    protected override void Start()
    {
        base.Start();
        mRigidbody = GetComponent<Rigidbody>();
        GameObject Effect = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/PhantomExp"), transform.position, Quaternion.identity);
        Destroy(Effect, 1.0f);
    }

    public override void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
    }

    protected override void FixedUpdate()
    {
        if (isCollide == false)
        {
            if (isEffect)
                return;
            Vector3 targetDir = (mTarget.transform.position - transform.position).normalized;
            Vector3 cross = Vector3.Cross(targetDir, transform.forward);

            mRigidbody.angularVelocity = cross * 200.0f;
            mRigidbody.velocity = targetDir * mSpeed * Time.deltaTime * 25.0f;

            if (Vector3.Distance(transform.position, mTarget.transform.position) < mMaximumDistance && !isCollide)
            {
                isCollide = true;
                GameObject damageEffect = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/PhathomExplosion")
, mTarget.transform.position + new Vector3(UnityEngine.Random.Range(1.0f, 5.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
Quaternion.identity, mTarget.transform);
                Destroy(damageEffect, 1.0f);
                mActionEvent?.Invoke();
                if (clip.Length > 0)
                    AudioManager.PlaySfx(clip[UnityEngine.Random.Range(0, clip.Length - 1)]);
                Destroy(this.gameObject, 1.0f);
            }
        }
    }
}
