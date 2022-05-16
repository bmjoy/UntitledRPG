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
    private GameObject mFirePos;
    private GameObject mCanvas;
    private Vector3 mPos = Vector3.zero;
    private Vector3 mTargetPos = Vector3.zero;

    private Animator mAnimator;
    private Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    [SerializeField]
    private float mWaitingTimeForBattle = 0.75f;
    [SerializeField]
    private Vector2 mFireLocation;
    public Unit_Setting Unit_Setting => mSetting;
    public Skill_DataBase mSkillDataBase;
    public SpriteRenderer mSpriteRenderer;

    private BuffAndNerfEntity mBuffNerfController;

    private TextMeshProUGUI mLevelText;
    private MiniHealthBar mHealthBar;
    private bool isGrounded = false;
    private bool isAIinitialized = false;
  
    public AIBuild mAiBuild;
    public Status mStatus;
    public Conditions mConditions;

    private Vector3 mVelocity = Vector3.zero;
    private GameObject mGroundCheck;
    private float mGroundDistance = 2.0f;
    
    [SerializeField]
    private float mAttackDistance= 0.0f;
    [HideInInspector]
    public float mMagicDistance= 0.0f;
    public GameObject mSelected = null;
    protected virtual void Start()
    {

    }

    public void Componenet_Initialize()
    {
        mStatus = new Status(mSetting.Level,
            mSetting.EXP,
            mSetting.Gold,
            mSetting.MaxHealth,
            mSetting.MaxHealth,
            mSetting.MaxMana,
            mSetting.MaxMana,
            mSetting.Attack,
            mSetting.Armor,
            mSetting.Magic_Resistance,
            mSetting.Defend,
            mSetting.Agility,
            mSetting.MagicPower);
        mConditions = new Conditions(false, false, false, false);

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mRigidbody.velocity = Vector3.zero;
        mBuffNerfController = (GetComponent<BuffAndNerfEntity>() != null) ?
            GetComponent<BuffAndNerfEntity>() : gameObject.AddComponent<BuffAndNerfEntity>();
        mSkillDataBase = GetComponent<Skill_DataBase>();
        GetComponent<BoxCollider>().enabled = true;
    }

    public void Prefab_Initialize()
    {
        if(mGroundCheck == null)
        {
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.position = new Vector3(transform.position.x,
                transform.position.y - (transform.GetComponent<BoxCollider>().size.y), transform.position.z);
            groundCheck.transform.parent = transform;
            mGroundCheck = groundCheck;
        }

        if (mType == AttackType.Range && mFirePos == null)
        {
            GameObject fire = new GameObject("Fire");
            fire.transform.position = new Vector3(transform.position.x + mFireLocation.x, transform.position.y + mFireLocation.y, transform.position.z);
            fire.transform.SetParent(transform);
            mFirePos = fire;
        }
        if(mCanvas != null)
        {
            Destroy(mCanvas);
            mCanvas = null;
        }
        mCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/CanvasForUnit"), transform.position, Quaternion.identity);
        mCanvas.transform.SetParent(transform);
        mSelected = mCanvas.transform.Find("Selected").gameObject;
        mHealthBar = mCanvas.transform.Find("Borader").Find("HealthBarPrefab").GetComponent<MiniHealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth, mStatus.mMana, mStatus.mMaxMana);
        mLevelText = mCanvas.transform.Find("Borader").Find("Text").GetComponent<TextMeshProUGUI>();
        mLevelText.text = mStatus.mLevel.ToString();
        mLevelText.gameObject.SetActive(true);
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        mHealthBar.Active(false);
        mSelected.SetActive(false);
    }

    public void AI_Initialize()
    {
        if (isAIinitialized)
            return;
        mAiBuild.actionEvent = ActionEvent.IntroWalk;

        mAiBuild.property = (AIProperty)UnityEngine.Random.Range(0, 2);
        mAiBuild.type = AIType.None;
        mAiBuild.stateMachine = (mAiBuild.stateMachine == null) ? gameObject.AddComponent<StateMachine>()
            : GetComponent<StateMachine>();
        mAiBuild.stateMachine.mAgent = this;
        mAiBuild.stateMachine.AddState<Waiting>(new Waiting(), "Waiting");
        mAiBuild.stateMachine.AddState<Standby>(new Standby(), "Standby");
        mAiBuild.stateMachine.AddState<AttackBehavior>(new AttackBehavior(), "Attack");
        mAiBuild.stateMachine.AddState<DefendBehavior>(new DefendBehavior(), "Defend");
        mAiBuild.stateMachine.ChangeState("Waiting");
        isAIinitialized = true;
    }

    protected virtual void Update()
    {
        if(mConditions.isDied == false)
            mLevelText.gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        mHealthBar.mCurrentMana = mStatus.mMana;

        switch (mAiBuild.actionEvent)
        {
            case ActionEvent.None:
                {
                    mAnimator.SetBool("Death", (mConditions.isDied) ? true : false);
                    if (mAiBuild.type == AIType.Auto)
                        mAiBuild.stateMachine.ActivateState();
                    transform.position = (Vector3.Distance(transform.position, mField.transform.position) > 0.5f) ?
    Vector3.MoveTowards(transform.position, mField.transform.position, Time.deltaTime * 7.0f) : transform.position;
                    mAnimator.SetFloat("Speed", Vector3.Distance(transform.position, mField.transform.position) > 0.5f ? 1.0f : 0.0f);
                }
                break;
            case ActionEvent.IntroWalk:
                {
                    mAiBuild.actionEvent = Run(mField.transform.position, 0.1f, ActionEvent.None, ActionEvent.IntroWalk);
                    mHealthBar.Active((mAiBuild.actionEvent == ActionEvent.None) ? true : false);
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
                    mAnimator.SetFloat("Speed", 0.0f);
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
        mVelocity.y = (isGrounded && mVelocity.y <= 0.0f) ? -GetComponent<BoxCollider>().size.y + 0.2f : mVelocity.y;
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
            while (true)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, (mFlag == Flag.Player) ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Player")))
                {
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    if (mTarget && Input.GetMouseButtonDown(0))
                        break;
                }
                else
                {
                    mTarget?.mSelected.SetActive(false);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    BattleManager.Instance.Cancel();
                    break;
                }
                yield return null;
            }
            UIManager.Instance.DisplayText(false);
            UIManager.Instance.ChangeText(UIManager.Instance.mTextForAccpet);
        }

        if (mConditions.isCancel == false && mTarget)
        {
            mSpriteRenderer.sortingOrder = (transform.position.z < mTarget?.transform.position.z) ? 3 : 4;
            mTarget.mSpriteRenderer.sortingOrder = (transform.position.z > mTarget?.transform.position.z) ? 3 : 4;

            onComplete?.Invoke();
            if (mType == AttackType.Melee)
            {
                mAiBuild.actionEvent = ActionEvent.AttackWalk;
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
                PlayAnimation("Attack");
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length / 3.0f);
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
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length / 3.0f);
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
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Defend"),
            new Vector3(transform.position.x,
            transform.position.y + GetComponent<BoxCollider>().size.y / 2.0f,
            transform.position.z), Quaternion.identity);
        Destroy(go, 1.5f);
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
            value = (value - mStatus.mMagic_Resistance <= 0.0f) ? 1.0f : value - mStatus.mMagic_Resistance;

        mStatus.mHealth -= value;
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        mHealthBar.StartCoroutine(mHealthBar.PlayBleed());
        

        if (mStatus.mHealth <= 0.0f)
        {
            mSelected.SetActive(false);
            mConditions.isDied = true;
            mLevelText.gameObject.SetActive(false);
            mStatus.mHealth = 0.0f;
            mHealthBar.mCurrentHealth = 0.0f;
            if (mFlag == Flag.Enemy)
            {
                Destroy(mHealthBar.gameObject, 3.0f);
                GameManager.s_TotalExp += mStatus.mEXP;
                GameManager.s_TotalGold += mStatus.mGold;
                GameManager.Instance.TotalSoul += GameManager.Instance.mAmountofSoul;
            }
            mAnimator.SetBool("Death",true);
            mHealthBar.ActiveDeathAnimation(true);
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
        {
            mTarget.mSelected.SetActive(false);
            mSpriteRenderer.sortingOrder = mTarget.mSpriteRenderer.sortingOrder = 4;
        }
        mTarget = null;
        mOrder = Order.TurnEnd;
        mAiBuild.stateMachine.ChangeState("Waiting");
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

    public void ClearBuffAndNerf()
    {
        mBuffNerfController?.Stop();
    }

    virtual public void ResetUnit()
    {
        Componenet_Initialize();
        Prefab_Initialize();
        AI_Initialize();
    }
}
