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
    private float mChannelingTime = 1.0f;
    [SerializeField]
    private float mMaximumDistance = 2.5f;
    private Unit mTarget;
    private DamageType mDamageType;
    private Action mActionEvent;
    private void Start()
    {
        mEffect = transform.GetChild(0).transform.gameObject;
        if (mEffect)
        {
            mEffect.transform.position = transform.position;
            if (mTarget.mFlag == Flag.Player)
            {
                mEffect.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
                transform.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                mEffect.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
                transform.GetComponent<SpriteRenderer>().flipX = false;
            }
            isEffect = true;
            StartCoroutine(WaitforSecond());
            mEffect.SetActive(true);
        }    
    }
    public void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
    }

    private void Update()
    {
        if(isCollide == false)
        {
            if (isEffect)
                return;
            transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, mSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, mTarget.transform.position) < mMaximumDistance)
            {
                isCollide = true;
                mActionEvent?.Invoke();
                Destroy(this.gameObject,0.1f);
            }
        }
    }

    private IEnumerator WaitforSecond()
    {
        yield return new WaitForSeconds(mChannelingTime);
        isEffect = false;
    }
}
