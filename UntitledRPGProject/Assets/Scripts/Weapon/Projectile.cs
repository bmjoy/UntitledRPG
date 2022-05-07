using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject mEffect;
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
        mEffect = transform.GetChild(0).transform.gameObject;
        if (mEffect)
        {
            mEffect.transform.eulerAngles = (mTarget.mFlag == Flag.Player) ? new Vector3(-90.0f, 0.0f, 0.0f) : new Vector3(90.0f, 0.0f, 0.0f);
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
            if (isEffect)
                return;
            transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, mSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, mTarget.transform.position) < 0.75f)
            {
                isCollide = true;
                mActionEvent?.Invoke();
                Destroy(this.gameObject, 1.0f);
            }
        }
    }

    private IEnumerator WaitforSecond()
    {
        yield return mTime;
        isEffect = false;
    }
}
