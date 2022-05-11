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
    public AttackType mType = AttackType.Melee;
    public Flag mFlag;

    public GameObject mField;
    private Vector3 mPos = Vector3.zero;
    private Vector3 mTargetPos = Vector3.zero;

    private Animator mAnimator;
    private Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    [SerializeField]
    public float mWaitingTimeForBattle = 0.75f;
    public Unit_Setting Unit_Setting => mSetting;
    public Skill_DataBase mSkillDataBase;
    public SpriteRenderer mSpriteRenderer;

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

    [SerializeField]
    private float mAttackDistance= 0.0f;
    [SerializeField]
    private float mMagicDistance= 0.0f;

    protected virtual void Start()
    {
        Componenet_Initialize();
        Prefab_Initialize();
        AI_Initialize();
    }

    public void Componenet_Initialize()
    {
        if(!GameManager.Instance.mUnitData.ContainsKey(mSetting.Name))
        {
            mStatus = new Status(mSetting.Level, mSetting.EXP, mSetting.Gold, mSetting.MaxHealth, mSetting.MaxHealth, mSetting.MaxMana, mSetting.MaxMana, mSetting.Attack, mSetting.Armor,
mSetting.Magic_Resistance, mSetting.Defend, mSetting.Agility, mSetting.MagicPower);
            mConditions = new Conditions(false, false, false, false, false);
        }

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mRigidbody.velocity = Vector3.zero;
        if(GetComponent<BuffAndNerfEntity>() != null)
           mBuffNerfController = GetComponent<BuffAndNerfEntity>();
        else
            mBuffNerfController = gameObject.AddComponent<BuffAndNerfEntity>();

        if (GetComponent<Skill_DataBase>())
            mSkillDataBase = GetComponent<Skill_DataBase>();
    }

    public void Prefab_Initialize()
    {
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.position = new Vector3(transform.position.x,
            transform.position.y - (transform.GetComponent<BoxCollider>().size.y), transform.position.z);
        groundCheck.transform.parent = transform;
        mGroundCheck = groundCheck;

        if (mType == AttackType.Range)
        {
            GameObject fire = new GameObject("Fire");
            fire.transform.position = new Vector3(transform.position.x, transform.position.y + transform.GetComponent<BoxCollider>().size.y / 2.0f, transform.position.z);
            fire.transform.parent = transform;
        }

        GameObject source = Instantiate(Resources.Load<GameObject>("Prefabs/CanvasForUnit"), transform.position, Quaternion.identity);
        source.transform.SetParent(transform);

        mHealthBar = source.transform.Find("Borader").Find("HealthBarPrefab").GetComponent<HealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth, mStatus.mMana, mStatus.mMaxMana);

        mLevelText = source.transform.Find("Borader").Find("Text").GetComponent<TextMeshProUGUI>();
        mLevelText.text = mStatus.mLevel.ToString();
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        mHealthBar.Active(false);
    }

    public void AI_Initialize()
    {
        mAiBuild.actionEvent = ActionEvent.IntroWalk;

        mAiBuild.property = (AIProperty)UnityEngine.Random.Range(0, 2);
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

        mHealthBar.mCurrentMana = mStatus.mMana;

        switch (mAiBuild.actionEvent)
        {
            case ActionEvent.None:
                {
                    ZeroVelocity();
                    if (mAiBuild.type == AIType.Auto)
                        mAiBuild.stateMachine.ActivateState();
                    if(Vector3.Distance(transform.position, mField.transform.position) > 0.2f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, mField.transform.position, Time.deltaTime * 7.0f);
                        mAnimator.SetFloat("Speed", 1.0f);
                    }
                    mAnimator.SetFloat("Speed", 0.0f);
                }
                break;
            case ActionEvent.IntroWalk:
                {
                    mAiBuild.actionEvent = Run(mField.transform.position, 0.1f, ActionEvent.None, ActionEvent.IntroWalk);
                    if (mAiBuild.actionEvent == ActionEvent.None)
                        mHealthBar.Active(true);

                    var q = Quaternion.LookRotation(Vector3.Normalize(mTargetPos - transform.position), Vector3.up);
                    q.x = q.z = 0.0f;
                    transform.rotation = q;
                }
                break;
            case ActionEvent.AttackWalk:
                mAiBuild.actionEvent = Run(mTarget.transform.position, mAttackDistance, ActionEvent.Busy, ActionEvent.AttackWalk);
                break;
            case ActionEvent.MagicWalk:
                mAiBuild.actionEvent = Run(mTarget.transform.position, mMagicDistance, ActionEvent.Busy, ActionEvent.MagicWalk);
                break;
            case ActionEvent.BackWalk:
                mAiBuild.actionEvent = Run(mField.transform.position, 0.1f, ActionEvent.Busy, ActionEvent.BackWalk);
                break;
            case ActionEvent.Busy:
                {
                    ZeroVelocity();
                    mAnimator.SetFloat("Speed", 0.0f);
                }
                break;
        }
        CheckGround();
    }

    private ActionEvent Run(Vector3 to, float maxDist, ActionEvent actionEvent1, ActionEvent actionEvent2)
    {
        transform.position = Vector3.MoveTowards(transform.position, to, Time.deltaTime * 7.0f);
        mAnimator.SetFloat("Speed", 1.0f);
        return ((Vector3.Distance(transform.position, to) < maxDist)) ? actionEvent1 : actionEvent2;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(mGroundCheck.transform.position, mGroundDistance, LayerMask.GetMask("Ground"));
        if (isGrounded && mVelocity.y <= 0.0f)
            mVelocity.y = -GetComponent<BoxCollider>().size.y + 0.2f;

        mVelocity.y += -9.8f * Time.deltaTime;
        mRigidbody.AddForce(mVelocity * Time.deltaTime);
        if (transform.position.y <= -50.0f)
            transform.position = new Vector3(mField.transform.position.x, mField.transform.position.y + 5.0f, mField.transform.position.z);
    }

    virtual public IEnumerator AttackAction(DamageType type, Action onComplete)
    {
        mConditions.isCancel = false;
        
        if (mAiBuild.type == AIType.Manual && mFlag == Flag.Player)
        {
            UIManager.Instance.ChangeText(UIManager.Instance.mTextForTarget);
            UIManager.Instance.DisplayText(true);
            while (mTarget == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, (mFlag == Flag.Player) ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player")))
                {
                    if(Input.GetMouseButtonDown(0) && hit.transform.GetComponent<Unit>().mConditions.isDied ==false)
                    {
                        mTarget = hit.transform.GetComponent<Unit>();
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
            UIManager.Instance.DisplayText(false);
            UIManager.Instance.ChangeText(UIManager.Instance.mTextForAccpet);
        }

        if (mConditions.isCancel == false)
        {
            if (transform.position.z < mTarget.transform.position.z)
                mSpriteRenderer.sortingOrder = 3;
            else
                mTarget.mSpriteRenderer.sortingOrder = 3;
            onComplete?.Invoke();
            if (mType == AttackType.Melee)
            {
                mAiBuild.actionEvent = ActionEvent.AttackWalk;
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
                PlayAnimation("Attack");
                yield return new WaitForSeconds(0.35f);
                if (mTarget)
                    mTarget.TakeDamage(mStatus.mDamage, type);
                yield return new WaitForSeconds(mWaitingTimeForBattle);
                mAiBuild.actionEvent = ActionEvent.BackWalk;
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
            }
            else
            {
                mAiBuild.actionEvent = ActionEvent.Busy;
                PlayAnimation("Attack");
                yield return new WaitForSeconds(0.35f);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mSetting.Name),transform.Find("Fire").position,Quaternion.identity);
                Bullet bullet = go.GetComponent<Bullet>();
                bullet.Initialize(mTarget, mStatus.mDamage);
                yield return new WaitUntil(() => bullet.isDamaged == true);
            }

            TurnEnded();
        }

    }

    virtual public IEnumerator DefendAction(Action onComplete)
    {
        mConditions.isDefend = true;
        onComplete?.Invoke();
        yield return new WaitForSeconds(mWaitingTimeForBattle);
        TurnEnded();
    }

    virtual public IEnumerator MagicAction(Action onComplete)
    {
        mSkillDataBase.Use();
        onComplete?.Invoke();
        yield return new WaitUntil(() => mSkillDataBase.Skill.isComplete == true);
        if (mSkillDataBase.Skill.isActive == false)
            BattleManager.Instance.Cancel();
        else
        {
            yield return new WaitForSeconds(mWaitingTimeForBattle);
            TurnEnded();
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
            value = (value + mStatus.mMagicPower - mStatus.mMagic_Resistance <= 0.0f) ? 1.0f : value + mStatus.mMagicPower - mStatus.mMagic_Resistance;
        mStatus.mHealth -= value;
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        mHealthBar.StartCoroutine(mHealthBar.PlayBleed());
        mRigidbody.velocity = mRigidbody.angularVelocity = Vector3.zero;
        if (mStatus.mHealth <= 0.0f)
        {
            mConditions.isDied = true;
            mHealthBar.mBorader.GetComponent<Animator>().Play("Death");
            mLevelText.gameObject.SetActive(false);

            if(mFlag == Flag.Enemy)
            {
                Destroy(mHealthBar.gameObject, 3.0f);
                GameManager.s_TotalExp += mStatus.mEXP;
                GameManager.s_TotalGold += mStatus.mGold;
                // TODO: Item;
            }
            mAnimator.SetBool("Death",true);
            GetComponent<BoxCollider>().enabled = false;
            UIManager.Instance.mOrderbar.GetComponent<OrderBar>().DequeueOrder(this);
            mBuffNerfController.Stop();
            mField.GetComponent<Field>().Picked(false);
        }
        mAnimator.SetTrigger("Hit");
    }

    virtual public void TakeRecover(float val)
    {
        mStatus.mHealth += val;
        if (mStatus.mHealth >= mStatus.mMaxHealth)
            mStatus.mHealth = mStatus.mMaxHealth;
    }
    virtual public void TurnEnded()
    {
        mConditions.isPicked = false;
        mAiBuild.actionEvent = ActionEvent.None;
        if(mTarget != null)
            mSpriteRenderer.sortingOrder = mTarget.mSpriteRenderer.sortingOrder = 4;
        mTarget = null;
        mOrder = Order.TurnEnd;
    }
    virtual public void DisableUI()
    {
        mHealthBar.Active(false);
    }

    virtual public void SetBuff(TimedBuff buff)
    {
        mBuffNerfController.AddBuff(buff);
    }

    virtual public void SetNerf(TimedNerf nerf)
    {
        mBuffNerfController.AddNerf(nerf);
    }

    public void BuffAndNerfTick()
    {
        mBuffNerfController?.Tick();
    }

    private void ZeroVelocity()
    {
        mRigidbody.velocity = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ZeroVelocity();
    }

    virtual public void ResetUnit()
    {
        mStatus = new Status(mSetting.Level, mSetting.EXP, mSetting.Gold, mSetting.MaxHealth, mSetting.MaxHealth, mSetting.MaxMana,mSetting.MaxMana, mSetting.Attack, mSetting.Armor,
mSetting.Magic_Resistance, mSetting.Defend, mSetting.Agility, mSetting.MagicPower);
        mConditions = new Conditions(false, false, false, false, false);
    }
}
