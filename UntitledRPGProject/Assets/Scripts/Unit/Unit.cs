using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    public Unit mTarget = null;
    public Order mOrder = Order.Standby;
    public Flag mFlag;
    public LayerMask mTargetMask;
    public Vector3 mFieldPos = Vector3.zero;
    public Vector3 mPos = Vector3.zero;
    public Vector3 mTargetPos = Vector3.zero;

    public Animator mAnimator;
    public Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    public WaitForSeconds mWaitingTime = new WaitForSeconds(1.0f);
    public Unit_Setting Unit_Setting => mSetting;
    public Skill_DataBase mSkillDataBase;

    private BuffAndNerfEntity mBuffNerfController;

    public GameObject mHealthBarPrefab;
    private HealthBar mHealthBar;

    public float yAxis = 0.0f;

    public AIBuild mAiBuild;
    public Status mStatus;
    public Conditions mConditions;

    protected virtual void Start()
    {
        mStatus = new Status(mSetting.MaxHealth, mSetting.MaxHealth, mSetting.MaxMana, mSetting.Attack, mSetting.Armor,
            mSetting.Magic_Resistance, mSetting.Defend, mSetting.Agility, mSetting.MagicPower);
        mConditions = new Conditions(false, false, false, false, false);

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.velocity = Vector3.zero;
        mBuffNerfController = GetComponent<BuffAndNerfEntity>();
        if(GetComponent<Skill_DataBase>())
            mSkillDataBase = GetComponent<Skill_DataBase>();

        mHealthBar = mHealthBarPrefab.GetComponent<HealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth);

        mAiBuild.actionEvent = ActionEvent.IntroWalk;

        mAiBuild.property = (AIProperty)UnityEngine.Random.Range(0,2);
        mAiBuild.type = AIType.None;

        mAiBuild.stateMachine = gameObject.AddComponent<StateMachine>();
        mAiBuild.stateMachine.mAgent = this;
        mAiBuild.stateMachine.AddState<Standby>(new Standby(), "Standby");
        mAiBuild.stateMachine.AddState<AttackBehavior>(new AttackBehavior(), "Attack");
        mAiBuild.stateMachine.AddState<DefendBehavior>(new DefendBehavior(), "Defend");
        mAiBuild.stateMachine.ChangeState("Standby");
    }

    protected virtual void Update()
    {
        if (mConditions.isDied)
            return;
        switch (mAiBuild.actionEvent)
        {
            case ActionEvent.None:
                {
                    if(mAiBuild.type == AIType.Auto)
                        mAiBuild.stateMachine.ActivateState();
                    mAnimator.SetFloat("Speed", 0.0f);
                    mHealthBar.Active(true);
                }
                break;
            case ActionEvent.IntroWalk:
                {
                    mHealthBar.Active(false);
                    mAiBuild.actionEvent = ((Vector3.Distance(transform.position, mPos) < 0.001f)) ? ActionEvent.None : ActionEvent.IntroWalk;
                    transform.position = Vector3.MoveTowards(transform.position, mPos, Time.deltaTime * mStatus.mAgility * 4.0f);

                    var q = Quaternion.LookRotation(Vector3.Normalize(mTargetPos - transform.position), Vector3.up);
                    q.x = q.z = 0.0f;
                    transform.rotation = q;
                    mAnimator.SetFloat("Speed", 1.0f);
                }
                break;
            case ActionEvent.AttackWalk:
                {
                    mAiBuild.actionEvent = ((Vector3.Distance(transform.position, mPos) < 2.0f)) ? ActionEvent.Busy : ActionEvent.AttackWalk;
                    transform.position = Vector3.MoveTowards(transform.position, mPos, Time.deltaTime * mStatus.mAgility * 4.0f);
                    mAnimator.SetFloat("Speed", 1.0f);
                }
                break;
            case ActionEvent.BackWalk:
                {
                    mAiBuild.actionEvent = ((Vector3.Distance(transform.position, mFieldPos) < 0.001f)) ? ActionEvent.Busy : ActionEvent.BackWalk;
                    transform.position = Vector3.MoveTowards(transform.position, mFieldPos, Time.deltaTime * mStatus.mAgility * 4.0f);
                    mAnimator.SetFloat("Speed", 1.0f);
                }
                break;
            case ActionEvent.Busy:
                mAnimator.SetFloat("Speed", 0.0f);
                break;
        }
    }

    virtual public IEnumerator AttackAction(DamageType type, Action onComplete)
    {
        mConditions.isCancel = false;
        if(mAiBuild.type == AIType.Manual && mFlag == Flag.Player)
        {
            UIManager.Instance.ChangeText_Target("Choose the Target");
            UIManager.Instance.DisplayAskingSkill(true);
            while (mTarget == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, mTargetMask))
                {
                    if(hit.transform.GetComponent<Unit>().mConditions.isDied ==false)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            mTarget = hit.transform.GetComponent<Unit>();
                            Debug.Log(hit.transform.name);
                        }
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        mConditions.isCancel = true;
                        BattleManager.Instance.Cancel();
                        break;
                    }
                }
                yield return null;
            }
            UIManager.Instance.DisplayAskingSkill(false);
            UIManager.Instance.ChangeText_Target("OK? (Y/N)");
        }

        if (mConditions.isCancel == false)
        {
            Debug.Log(this.Unit_Setting.Name + " Attacks");
            SetPosition(mTarget.transform.position, mTargetPos, ActionEvent.AttackWalk);
            yield return new WaitUntil(()=> mAiBuild.actionEvent == ActionEvent.Busy);
            PlayAnimation("Attack");
            if (mTarget)
            {
                Debug.Log(this.Unit_Setting.Name + " Attacks to " + mTarget.Unit_Setting.Name);
                mTarget.TakeDamage(mStatus.mDamage, type);
            }
            else
                Debug.Log("No Target");
            yield return mWaitingTime;
            SetPosition(mFieldPos, mTargetPos, ActionEvent.BackWalk);
            yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
            onComplete?.Invoke();
            TurnEnded();
            mAiBuild.actionEvent = ActionEvent.None;
        }

    }

    virtual public IEnumerator DefendAction(Action onComplete)
    {
        mConditions.isDefend = true;
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

    virtual public void PlayAnimation(string name)
    {
        //mAnimator.SetBool(name, active);
        mAnimator.Play(name);
    }

    virtual public void TakeDamage(float dmg, DamageType type)
    {
        if (mConditions.isDied)
            return;
        float value = dmg;
        if (type == DamageType.Physical)
            value = (mConditions.isDefend) ? dmg - (dmg * mStatus.mDefend / 100.0f) : dmg;

        mStatus.mHealth = (type == DamageType.Physical) ? mStatus.mHealth = mStatus.mHealth - 
            (value - mStatus.mArmor) : mStatus.mHealth - (value - mStatus.mMagic_Resistance);
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        Debug.Log("Takes " + value + " Damages");
        if (mStatus.mHealth <= 0)
        {
            mConditions.isDied = true;
            Destroy(mHealthBar.gameObject);
            Debug.Log(name + " Dead!");
        }

    }

    virtual public void TakeRecover(float val)
    {
        mStatus.mHealth += val;
        if (mStatus.mHealth >= mStatus.mMaxHealth)
            mStatus.mHealth = mStatus.mMaxHealth;
        Debug.Log(name + " Healed!");
    }
    virtual public void TurnEnded()
    {
        mConditions.isPicked = false;
        mTarget = null;
        mOrder = Order.TurnEnd;
    }

    virtual public void SetPosition(Vector3 pos, Vector3 targetPos, ActionEvent _actionEvent)
    {
        mPos = pos + new Vector3(0.0f, yAxis, 0.0f);
        mTargetPos = targetPos + new Vector3(0.0f, yAxis, 0.0f);
        mAiBuild.actionEvent = _actionEvent;
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
