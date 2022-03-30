using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject mEffect;
    public bool isCollide = false;
    [SerializeField]
    private float mSpeed = 10.0f;
    private Unit mTarget;
    private DamageType mDamageType;
    private Action mActionEvent;
    private Vector3 mDirection;
    private void Start()
    {
        if (mEffect)
            mEffect.SetActive(true);
    }
    public void Initialize(Unit target, Vector3 dir, DamageType damageType, Action actionEvent)
    {
        mTarget = target;
        mDirection = dir;
        mDamageType = damageType;
        mActionEvent += actionEvent;
    }

    private void Update()
    {
        if(isCollide == false)
        {
            transform.position += mDirection * mSpeed * Time.deltaTime;
            if(Vector3.Distance(transform.position, mTarget.transform.position) < 0.75f)
            {
                isCollide = true;
                mActionEvent?.Invoke();
                Destroy(this.gameObject, 1.0f);
            }
        }
    }
}
