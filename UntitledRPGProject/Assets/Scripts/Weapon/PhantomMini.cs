using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomMini : Phantom
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
    }

    protected override void Update()
    {
        if (isCollide == false)
        {
            if (isEffect)
                return;
            transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, mSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, mTarget.transform.position) < mMaximumDistance && !isCollide)
            {
                isCollide = true;
                int random = UnityEngine.Random.Range(1, 2);
                GetComponent<Animator>().Play((mAnimationCount >= 3) ? "Burst" + UnityEngine.Random.Range(1, 2) : "Burst");
                mActionEvent?.Invoke();
                Destroy(this.gameObject, 1.0f);
            }
            else
                transform.position += new Vector3(0.0f, UnityEngine.Random.Range(-0.3f, 0.3f), 0.0f);
        }

    }
}
