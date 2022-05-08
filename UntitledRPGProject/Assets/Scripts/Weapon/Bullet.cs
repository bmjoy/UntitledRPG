using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Animator mAnimator;
    private Unit mTarget;
    private float mPower = 0.0f;
    public float mSpeed = 0.0f;
    private int mCount = 0;
    private bool mInitialize = false;
    public bool isDamaged = false;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        mCount = mAnimator.runtimeAnimatorController.animationClips.Length;
    }

    public void Initialize(Unit target, float power)
    {
        mTarget = target;
        mPower = (power > 0) ? power : 10.0f;
        mInitialize = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(mInitialize)
            transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, Time.deltaTime * mSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Unit>() == mTarget)
        {
            mTarget.transform.GetComponent<Rigidbody>().velocity = mTarget.transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            mTarget.TakeDamage(mPower, DamageType.Physical);

            int random = Random.Range(1, 2);
            mAnimator.Play((mCount >= 3) ? "Burst" + Random.Range(1, 2) : "Burst");
            isDamaged = true;
            Destroy(this.gameObject, 2.5f);
        }
    }
}
