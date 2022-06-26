using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBullet : Bullet
{
    [SerializeField]
    private GameObject mExplosion;
    private Vector3 mDirection;

    public override void Initialize(Transform target, float power)
    {
        mTarget = target;
        mPower = (power > 0) ? power : 10.0f;
        mDirection = (target.position - transform.position).normalized;
        mInitialize = true;
    }

    protected override void FixedUpdate()
    {
        if (mInitialize)
            GetComponent<Rigidbody>().AddForce(mDirection * mSpeed);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (isDamaged)
            return;
        if (collision.collider.GetComponent<Unit>() == mTarget.GetComponent<Unit>())
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
