using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject mEffect;
    public bool isCollide = false;
    private bool isEffect = false;
    [SerializeField]
    private float mSpeed = 10.0f;
    [SerializeField]
    private WaitForSeconds mTime = new WaitForSeconds(1.0f);
    private Unit mTarget;
    private DamageType mDamageType;
    private Action mActionEvent;
    private Vector3 mDirection;
    private void Start()
    {
        if (mEffect)
        {
            isEffect = true;
            StartCoroutine(WaitforSecond());
            mEffect.SetActive(true);
        }    
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
            if(isEffect == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, mSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, mTarget.transform.position) < 0.75f)
                {
                    isCollide = true;
                    mActionEvent?.Invoke();
                    Destroy(this.gameObject, 1.0f);
                }
            }
            else
                transform.position = new Vector3(Mathf.Cos(Time.time * 0.5f * mSpeed), transform.position.y, transform.position.z);
        }
    }

    private IEnumerator WaitforSecond()
    {
        yield return mTime;
        isEffect = false;
        transform.LookAt(mDirection);
    }
}
