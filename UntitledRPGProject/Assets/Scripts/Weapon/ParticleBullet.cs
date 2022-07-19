using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBullet : Bullet
{
    [SerializeField]
    private GameObject mExplosion;
    private Vector3 mDirection;
    private Rigidbody mRigidbody;
    public override void Initialize(Transform target, float power)
    {
        mRigidbody = GetComponent<Rigidbody>();
        mTarget = target;
        mPower = (power > 0) ? power : 10.0f;
        mDirection = (target.position - transform.position).normalized;
        isDamaged = false;
        mInitialize = true;
    }

    protected override void FixedUpdate()
    {
        if (mInitialize)
        {
            Vector3 targetDir = (mTarget.transform.position - transform.position).normalized;
            Vector3 cross = Vector3.Cross(targetDir, transform.forward);

            mRigidbody.angularVelocity = cross * 200.0f;
            mRigidbody.velocity = targetDir * mSpeed * Time.deltaTime * 25.0f;
        }

        if (Vector3.Distance(transform.position, mTarget.transform.position) < mMaximumDistance && !isDamaged)
        {
            isDamaged = true;
            mTarget.transform.GetComponent<Rigidbody>().velocity = mTarget.transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            mTarget.GetComponent<Unit>().TakeDamage(mPower, DamageType.Physical);

            AudioManager.PlaySfx(clip);
            GameObject damage = Instantiate(mExplosion, mTarget.GetComponent<Unit>().transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            Destroy(damage, 1.5f);
            Destroy(this.gameObject);
        }
    }
}
