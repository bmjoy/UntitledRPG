using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    public Unit mTarget = null;
    public Order mOrder = Order.Standby;
    public Flag mFlag;

    public Vector3 mFieldPos = Vector3.zero;
    private Vector3 mPos = Vector3.zero;
    private Vector3 mTargetPos = Vector3.zero;

    private Animator mAnimator;
    private Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    public WaitForSeconds mWaitingTime = new WaitForSeconds(0.75f);
    public Unit_Setting Unit_Setting => mSetting;
    public Skill_DataBase mSkillDataBase;

    private BuffAndNerfEntity mBuffNerfController;

    private TextMeshProUGUI mLevelText;
    private HealthBar mHealthBar;
    private bool isGrounded = false;
  
    public AIBuild mAiBuild;
    public Status mStatus;
    public Conditions mConditions;

    private Vector3 mVelocity = Vector3.zero;
    private GameObject mGroundCheck;
    private float mGroundDistance = 2.0f;

    protected virtual void Start()
    {
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.position = new Vector3(transform.position.x,
            transform.position.y - (transform.GetComponent<BoxCollider>().size.y + 0.1f), transform.position.z);
        groundCheck.transform.parent = transform;
        mGroundCheck = groundCheck;

        mStatus = new Status(mSetting.Level, mSetting.EXP, mSetting.Gold, mSetting.MaxHealth, mSetting.MaxHealth, mSetting.MaxMana, mSetting.Attack, mSetting.Armor,
            mSetting.Magic_Resistance, mSetting.Defend, mSetting.Agility, mSetting.MagicPower);
        mConditions = new Conditions(false, false, false, false, false);

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.velocity = Vector3.zero;
        mBuffNerfController = GetComponent<BuffAndNerfEntity>();
        if(GetComponent<Skill_DataBase>())
            mSkillDataBase = GetComponent<Skill_DataBase>();

        GameObject source = Instantiate(Resources.Load<GameObject>("Prefabs/CanvasForUnit"), transform.position,Quaternion.identity);
        source.transform.SetParent(transform);

        mHealthBar = source.transform.Find("Borader").Find("HealthBarPrefab").GetComponent<HealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth);

        mLevelText = source.transform.Find("Borader").Find("Text").GetComponent<TextMeshProUGUI>();
        mLevelText.text = mStatus.mLevel.ToString();
        mHealthBar.Active(false);
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
        else
            mLevelText.gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        isGrounded = Physics.CheckSphere(mGroundCheck.transform.position, mGroundDistance, 3);
        switch (mAiBuild.actionEvent)
        {
            case ActionEvent.None:
                {
                    mRigidbody.velocity = Vector3.zero;
                    mRigidbody.angularVelocity = Vector3.zero;
                    if(mAiBuild.type == AIType.Auto)
                        mAiBuild.stateMachine.ActivateState();
                    mAnimator.SetFloat("Speed", 0.0f);
                }
                break;
            case ActionEvent.IntroWalk:
                {
                    mAiBuild.actionEvent = ((Vector3.Distance(transform.position, mPos) < 2.0f)) ? ActionEvent.None : ActionEvent.IntroWalk;
                    if(mAiBuild.actionEvent == ActionEvent.None)
                        mHealthBar.Active(true);
                    transform.position = Vector3.MoveTowards(transform.position, mPos, Time.deltaTime * 7.0f);

                    var q = Quaternion.LookRotation(Vector3.Normalize(mTargetPos - transform.position), Vector3.up);
                    q.x = q.z = 0.0f;
                    transform.rotation = q;
                    mAnimator.SetFloat("Speed", 1.0f);
                }
                break;
            case ActionEvent.AttackWalk:
                {
                    mAiBuild.actionEvent = ((Vector3.Distance(transform.position, mPos) < 2.0f)) ? ActionEvent.Busy : ActionEvent.AttackWalk;
                    transform.position = Vector3.MoveTowards(transform.position, mPos, Time.deltaTime * 7.0f);
                    mAnimator.SetFloat("Speed", 1.0f);
                }
                break;
            case ActionEvent.BackWalk:
                {
                    mAiBuild.actionEvent = ((Vector3.Distance(transform.position, mFieldPos) < 1.0f)) ? ActionEvent.Busy : ActionEvent.BackWalk;
                    transform.position = Vector3.MoveTowards(transform.position, mFieldPos, Time.deltaTime * 7.0f);
                    mAnimator.SetFloat("Speed", (mAiBuild.actionEvent == ActionEvent.BackWalk) ? 1.0f : 0.0f);
                }
                break;
            case ActionEvent.Busy:
                {
                    mRigidbody.velocity = Vector3.zero;
                    mRigidbody.angularVelocity = Vector3.zero;
                    mAnimator.SetFloat("Speed", 0.0f);
                }
                break;
        }
        if (isGrounded && mVelocity.y <= 0.0f)
            mVelocity.y = -GetComponent<BoxCollider>().size.y + 0.2f;

        mVelocity.y += -9.8f * Time.deltaTime;
        mRigidbody.AddForce(mVelocity * Time.deltaTime);
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
                if (Physics.Raycast(ray, out hit, 100, (mFlag == Flag.Player) ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player")))
                {
                    if(hit.transform.GetComponent<Unit>().mConditions.isDied ==false)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            mTarget = hit.transform.GetComponent<Unit>();
                            Debug.Log(hit.transform.name);
                        }
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    mConditions.isCancel = true;
                    BattleManager.Instance.Cancel();
                    break;
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
            yield return new WaitForSeconds(0.35f);
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

        if (mSkillDataBase.mSkill.isActive == false)
            BattleManager.Instance.Cancel();
        else
        {
            yield return mWaitingTime;
            onComplete?.Invoke();
            TurnEnded();
            Debug.Log("Turn End!");
        }
    }

    virtual public void PlayAnimation(string name)
    {
        mAnimator.Play(name);
    }

    virtual public void TakeDamage(float dmg, DamageType type)
    {
        if (mConditions.isDied)
            return;
        float value = dmg;
        if (type == DamageType.Physical)
        {
            value = (mConditions.isDefend) ? dmg - (dmg * mStatus.mDefend / 100.0f) : dmg;
            value = (value - mStatus.mArmor <= 0.0f) ? 1.0f : value - mStatus.mArmor;
        }
        else
            value = (value - mStatus.mMagic_Resistance <= 0.0f) ? 1.0f : value - mStatus.mMagic_Resistance;
        mStatus.mHealth -= value;
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        Debug.Log("Takes " + value + " Damages" + ", " + mStatus.mHealth + " lefts");
        if (mStatus.mHealth <= 0.0f)
        {
            mConditions.isDied = true;
            Destroy(mLevelText.gameObject);
            Destroy(mHealthBar.gameObject);

            if(mFlag == Flag.Enemy)
            {
                GameManager.Instance.s_TotalExp += mStatus.mEXP;
                GameManager.Instance.s_TotalGold += mStatus.mGold;
                // TODO: Item;
            }

            Debug.Log(name + " Dead!");
            mAnimator.SetBool("Death",true);
            UIManager.Instance.mOrderbar.GetComponent<OrderBar>().DequeueOrder(this);
        }
        mAnimator.SetTrigger("Hit");
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
        mPos = pos;
        mTargetPos = targetPos;
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

    private void OnCollisionEnter(Collision collision)
    {
        mRigidbody.velocity = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;
    }

    public void DisableUI()
    {
        mHealthBar.Active(false);
    }
}
