using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    public float mHealth = 0.0f;
    public float mMana = 0.0f;
    public float mDamage = 0.0f;
    public float mArmor = 0.0f;
    public float mMagic_Resistance = 0.0f;
    public float mDefend = 0.0f;
    public float mAgility = 0.0f;

    public bool isPicked = false;
    public bool isDied = false;
    public bool isDefend = false;
    public bool isMove = false;

    public Unit mTarget = null;
    public Order mOrder = Order.Standby;
    public Flag mFlag;
    public Vector3 mPos = Vector3.zero;
    public Vector3 mTargetPos = Vector3.zero;

    public Animator mAnimator;
    public Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    public Unit_Setting Unit_Setting => mSetting;

    private float currentTime = 0.0f;
    private float mReadyTime = 2.5f;

    protected virtual void Start()
    {
        mHealth = mSetting.MaxHealth;
        mMana = mSetting.MaxMana;
        mDamage = mSetting.Attack;
        mArmor = mSetting.Armor;
        mMagic_Resistance = mSetting.Magic_Resistance;
        mDefend = mSetting.Defend;
        mAgility = mSetting.Agility;
        isDied = false;
        isPicked = false;
        isDefend = false;
        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (isMove)
        {
            if (Vector3.Distance(transform.position, mPos) < 0.01f)
            {
                var q = Quaternion.LookRotation(Vector3.Normalize(mTargetPos - transform.position), Vector3.up);
                q.x = 0.0f;
                q.z = 0.0f;
                transform.rotation = q;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, mPos, Time.deltaTime * mAgility * 5.5f);
            }
            currentTime += Time.deltaTime;
            if (currentTime >= mReadyTime)
            {
                mRigidbody.velocity = Vector3.zero;
                currentTime = 0.0f;
                isMove = false;
            }
        }
    }

    virtual public IEnumerator AttackAction(Unit opponent, DamageType type)
    {
        yield return new WaitForSeconds(1.0f);
        opponent.TakeDamage(mDamage, type);
    }

    virtual public void PlayAnimation(string name, bool active)
    {
        mAnimator.SetBool(name, active);
    }

    virtual public void TakeDamage(float dmg, DamageType type)
    {
        if (isDied)
            return;
        float value = 0.0f;
        if (type == DamageType.Physical)
        {
            if(isDefend)
                value = dmg - (dmg * mDefend * 100.0f);
            else
                value = dmg;

            mHealth = mHealth - (value - mArmor);
            if (mHealth <= 0)
                isDied = true;
        }
        else
        {
            mHealth = mHealth - (dmg - mMagic_Resistance);
            if (mHealth <= 0)
                isDied = true;
        }
    }

    virtual public void TakeRecover(float val)
    {
        mHealth += val;
        if (mHealth >= mSetting.MaxHealth)
            mHealth = mSetting.MaxHealth;
    }
    virtual public void TurnEnded()
    {
        isPicked = false;
        mTarget = null;
        mOrder = Order.TurnEnd;
    }

    virtual public void SetPosition(Vector3 pos, Vector3 targetPos)
    {
        mPos = pos;
        mTargetPos = targetPos;
        isMove = true;
    }
}
