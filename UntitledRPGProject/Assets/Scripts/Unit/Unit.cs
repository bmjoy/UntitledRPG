using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float mMagicPower = 0.0f;

    public bool isPicked = false;
    public bool isDied = false;
    public bool isDefend = false;
    public bool isMove = false;
    public bool isCancel = false;

    public Unit mTarget = null;
    public Order mOrder = Order.Standby;
    public Flag mFlag;
    public LayerMask mTargetMask;
    public Vector3 mPos = Vector3.zero;
    public Vector3 mTargetPos = Vector3.zero;

    public Animator mAnimator;
    public Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    public WaitForSeconds mWaitingTime = new WaitForSeconds(1.0f);
    public Unit_Setting Unit_Setting => mSetting;
    public Skill_DataBase mSkillDataBase;

    private BuffAndNerfEntity mBuffNerfController;

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
        mMagicPower = mSetting.MagicPower;
        isDied = false;
        isPicked = false;
        isDefend = false;
        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mBuffNerfController = GetComponent<BuffAndNerfEntity>();
        if(GetComponent<Skill_DataBase>())
            mSkillDataBase = GetComponent<Skill_DataBase>();
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

    virtual public IEnumerator AttackAction(DamageType type, Action onComplete)
    {
        isCancel = false;
        UIManager.Instance.ChangeText_Target("Choose the Target");
        UIManager.Instance.DisplayAskingSkill(true);
        while (mTarget == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mTargetMask))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mTarget = hit.transform.GetComponent<Unit>();
                    Debug.Log(hit.transform.name);
                }
                if(Input.GetMouseButtonDown(1))
                {
                    isCancel = true;
                    BattleManager.Instance.Cancel();
                    break;
                }
            }
            yield return null;
        }
        UIManager.Instance.DisplayAskingSkill(false);
        UIManager.Instance.ChangeText_Target("OK? (Y/N)");
        if (isCancel == false)
        {
            Debug.Log(this.Unit_Setting.Name + " Attacks");
            yield return mWaitingTime;
            if (mTarget)
            {
                Debug.Log(this.Unit_Setting.Name + " Attacks to " + mTarget.Unit_Setting.Name);
                mTarget.TakeDamage(mDamage, type);
            }
            else
                Debug.Log("No Target");
            onComplete?.Invoke();
            TurnEnded();
        }

    }

    virtual public IEnumerator DefendAction(Action onComplete)
    {
        isDefend = true;
        //TODO: Effect?
        yield return mWaitingTime;
        onComplete?.Invoke();
        TurnEnded();
    }

    virtual public IEnumerator MagicAction(Action onComplete)
    {
        mSkillDataBase.Use();
        yield return new WaitUntil(() => mSkillDataBase.mSkill.isComplete == true);
        if(mSkillDataBase.mSkill.isActive == false)
            BattleManager.Instance.Cancel();
        else
        {
            yield return mWaitingTime;
            onComplete?.Invoke();
            TurnEnded();
        }

    }

    virtual public void PlayAnimation(string name, bool active)
    {
        mAnimator.SetBool(name, active);
    }

    virtual public void TakeDamage(float dmg, DamageType type)
    {
        if (isDied)
            return;
        float value = dmg;
        if (type == DamageType.Physical)
        {
            if(isDefend)
                value = dmg - (dmg * mDefend * 100.0f);
        }
        mHealth = (type == DamageType.Physical) ? mHealth = mHealth - (value - mArmor) : mHealth - (value - mMagic_Resistance);
        Debug.Log("Takes " + value + " Damages");
        if (mHealth <= 0)
        {
            isDied = true;
            Debug.Log(name + " Dead!");
        }

    }

    virtual public void TakeRecover(float val)
    {
        mHealth += val;
        if (mHealth >= mSetting.MaxHealth)
            mHealth = mSetting.MaxHealth;
        Debug.Log(name + " Healed!");
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

    virtual public void SetBuff(TimedBuff buff)
    {
        mBuffNerfController.AddBuff(buff);
    }

    virtual public void SetNerf(TimedNerf nerf)
    {
        mBuffNerfController.AddNerf(nerf);
    }
}
