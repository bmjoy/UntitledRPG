using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBullet : Bullet
{
    private Animator mAnimator;
    private int mCount = 0;
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        mCount = mAnimator.runtimeAnimatorController.animationClips.Length;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.collider.GetComponent<Unit>() == mTarget.GetComponent<Unit>())
        {
            int random = Random.Range(1, 2);
            mAnimator.Play((mCount >= 3) ? "Burst" + Random.Range(1, 2) : "Burst");
        }

    }
}
