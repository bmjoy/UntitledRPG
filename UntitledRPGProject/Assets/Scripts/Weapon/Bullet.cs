using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected Transform mTarget;
    protected float mPower = 0.0f;
    [SerializeField]
    protected float mSpeed = 0.0f;

    protected bool mInitialize = false;
    public bool isDamaged = false;

    public AudioClip clip;

    virtual public void Initialize(Transform target, float power)
    {
        mTarget = target;
        mPower = (power > 0) ? power : 10.0f;
        mInitialize = true;
    }

    virtual protected void FixedUpdate()
    {
        if(mInitialize)
            transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, Time.deltaTime * mSpeed);
    }

    virtual protected void OnCollisionEnter(Collision collision)
    {
        if (isDamaged)
            return;
        if (collision.collider.GetComponent<Unit>() == mTarget.GetComponent<Unit>())
        {
            isDamaged = true;
            mTarget.transform.GetComponent<Rigidbody>().velocity = mTarget.transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            mTarget.GetComponent<Unit>().TakeDamage(mPower, DamageType.Physical);

            AudioManager.PlaySfx(clip);
            Destroy(this.gameObject, 2.5f);
        }
    }
}
