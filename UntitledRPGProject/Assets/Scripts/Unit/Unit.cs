using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour, IUnit
{
    [HideInInspector]
    public Unit mTarget = null;
    [HideInInspector]
    public Order mOrder = Order.Standby;
    [HideInInspector]
    public GameObject mField;
    [HideInInspector]
    public GameObject mSelected = null;

    public AttackType mType = AttackType.Melee;
    public Flag mFlag;
    private GameObject mFirePos;
    private GameObject mCanvas;

    [HideInInspector]
    public Animator mAnimator;
    private Rigidbody mRigidbody;
    public Unit_Setting mSetting;
    [SerializeField]
    private Vector2 mFireLocation;
    public Unit_Setting Unit_Setting => mSetting;
    [HideInInspector]
    public Skill_DataBase mSkillDataBase;
    [HideInInspector]
    public SpriteRenderer mSpriteRenderer;

    [HideInInspector]
    public BuffAndNerfEntity mBuffNerfController;
    protected InventroySystem mInventroySystem;

    protected TextMeshProUGUI mLevelText;
    protected MiniHealthBar mHealthBar;
    private bool isGrounded = false;
    private bool isAIinitialized = false;
  
    public AIBuild mAiBuild;
    public Status mStatus;
    public BonusStatus mBonusStatus;
    public Conditions mConditions;

    private Vector3 mVelocity = Vector3.zero;
    private GameObject mGroundCheck;
    private float mGroundDistance = 2.0f;
    
    [SerializeField]
    protected float mAttackDistance= 0.0f;
    [HideInInspector]
    public float mMagicDistance = 0.0f;

    public Action mActionTrigger = null;
    public Action mStartActionTrigger = null;
    private float mTime = 0.0f;
    private float mWalkTime = 0.0f;
    private float mMaxWalkTime = 0.3f;

    private List<AudioClip> mRunClips = new List<AudioClip>();

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
            mSetting.MagicPower,
            mSetting.WeaponType);
        mConditions = new Conditions(false, false, false, false);

        mBonusStatus = new BonusStatus();
        mBonusStatus.mAgility = mBonusStatus.mArmor = mBonusStatus.mDefend = mBonusStatus.mDamage = mBonusStatus.mMana
            = mBonusStatus.mMagic_Resistance = mBonusStatus.mHealth = mBonusStatus.mMagicPower = 0.0f;

        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mRigidbody.velocity = Vector3.zero;
        mBuffNerfController = (GetComponent<BuffAndNerfEntity>() != null) ?
            GetComponent<BuffAndNerfEntity>() : gameObject.AddComponent<BuffAndNerfEntity>();
        mInventroySystem = (GetComponent<InventroySystem>() != null) ?
            gameObject.GetComponent<InventroySystem>() : gameObject.AddComponent<InventroySystem>();
        mInventroySystem.Initialize();
        mSkillDataBase = GetComponent<Skill_DataBase>();
        GetComponent<BoxCollider>().enabled = true;
    }

    public void Prefab_Initialize()
    {
        if(mGroundCheck == null)
        {
            GameObject groundCheck = Instantiate(Resources.Load<GameObject>("Prefabs/UnitGroundCheck"),(
             new Vector3(transform.position.x,
                transform.position.y - (transform.GetComponent<BoxCollider>().size.y / 2.0f), transform.position.z)),Quaternion.identity);
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
        mCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/CanvasForUnit"), transform.position
            + new Vector3(0.0f,GetComponent<BoxCollider>().center.y + 0.5f, 0.0f), Quaternion.identity);
        mCanvas.transform.SetParent(transform);
        mCanvas.GetComponent<Canvas>().sortingOrder = 8;
        mSelected = mCanvas.transform.Find("Selected").gameObject;
        mHealthBar = mCanvas.transform.Find("Borader").Find("HealthBarPrefab").GetComponent<MiniHealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth, mStatus.mMana, mStatus.mMaxMana);
        mLevelText = mCanvas.transform.Find("Borader").Find("Text").GetComponent<TextMeshProUGUI>();
        mLevelText.text = mStatus.mLevel.ToString();
        mLevelText.gameObject.SetActive(true);
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        mHealthBar.Active(false);
        mSelected.SetActive(false);

        mRunClips.Clear();
        if(mSetting.Clips.Count > 0)
        {
            mRunClips.Add(mSetting.Clips.Find(s => s.Type == SoundClip.SoundType.Run0).Clip);
            mRunClips.Add(mSetting.Clips.Find(s => s.Type == SoundClip.SoundType.Run1).Clip);
            mRunClips.Add(mSetting.Clips.Find(s => s.Type == SoundClip.SoundType.Run2).Clip);
        }

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
        mAiBuild.stateMachine.AddState<State>(new Waiting(), "Waiting");
        mAiBuild.stateMachine.AddState<State>(new Standby(), "Standby");
        mAiBuild.stateMachine.AddState<State>(new AttackBehavior(), "Attack");
        mAiBuild.stateMachine.AddState<State>(new DefendBehavior(), "Defend");
        mAiBuild.stateMachine.AddState<State>(new MagicBehavior(), "Magic");
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
        
        if(mAnimator.GetFloat("Speed") > 0.1f && mRunClips.Count > 0)
        {
            mWalkTime += Time.deltaTime;
            if(mWalkTime >= mMaxWalkTime)
            {
                AudioManager.PlaySfx(mRunClips[Random.Range(0, mRunClips.Count - 1)], 0.6f);
                mWalkTime = 0.0f;
            }
        }
    }

    protected ActionEvent Run(Vector3 to, float maxDist, ActionEvent actionEvent1, ActionEvent actionEvent2)
    {
        transform.position = Vector3.MoveTowards(transform.position, to, Time.deltaTime * 7.0f);
        mAnimator.SetFloat("Speed", 1.0f);
        return ((Vector3.Distance(transform.position, to) < maxDist)) ? actionEvent1 : actionEvent2;
    }

    protected void CheckGround()
    {
        isGrounded = Physics.CheckSphere(mGroundCheck.transform.position, mGroundDistance, LayerMask.GetMask("Ground"));
        mVelocity.y = (isGrounded && mVelocity.y <= 0.0f) ? -GetComponent<BoxCollider>().size.y + 0.2f : mVelocity.y;
        mVelocity.y += -9.8f * Time.deltaTime;
        mRigidbody.AddForce(mVelocity * Time.deltaTime);
        if (transform.position.y <= -50.0f)
            transform.position = new Vector3(mField.transform.position.x, mField.transform.position.y + 5.0f, mField.transform.position.z);
    }
    virtual public KeyValuePair<bool, BonusStatus> LevelUP()
    {
        BonusStatus bonus = new BonusStatus();
        if (mStatus.mEXP >= GameManager.Instance.mRequiredEXP + (50.0f * mStatus.mLevel))
        {
            bonus.mHealth += Random.Range(5, 10);
            bonus.mMana += Random.Range(5, 10);
            bonus.mDamage = Random.Range(1, 5);
            bonus.mMagicPower = Random.Range(1, 5);
            bonus.mArmor = Random.Range(1, 5);
            bonus.mMagic_Resistance = Random.Range(1, 5);
            mStatus.mLevel++;
            mStatus.mMaxHealth += bonus.mHealth;
            mStatus.mMaxMana += bonus.mMana;
            mStatus.mDamage += bonus.mDamage;
            mStatus.mMagicPower += bonus.mMagicPower;
            mStatus.mArmor += bonus.mArmor;
            mStatus.mMagic_Resistance += bonus.mMagic_Resistance;
            return new KeyValuePair<bool, BonusStatus>(true, bonus);
        }
        return new KeyValuePair<bool, BonusStatus>(false, bonus);
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
                if (Physics.Raycast(ray, out hit, 500, (mFlag == Flag.Player) ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Ally")))
                {
                    mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                    mTarget?.mSelected.SetActive(true);
                    if (mTarget && Input.GetMouseButtonDown(0))
                        break;
                }
                else
                    mTarget?.mSelected.SetActive(false);
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
            mTarget.mSelected.SetActive(false);
            mSpriteRenderer.sortingOrder = (transform.position.z < mTarget?.transform.position.z) ? 3 : 4;
            mTarget.mSpriteRenderer.sortingOrder = (transform.position.z > mTarget?.transform.position.z) ? 3 : 4;

            onComplete?.Invoke();
            if (mType == AttackType.Melee)
            {
                mAiBuild.actionEvent = ActionEvent.AttackWalk;
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
                
                PlayAnimation("Attack");
                mTime = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (mTarget)
                {
                    if(mActionTrigger != null)
                    {
                        mTime -= mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime / 3.0f;
                        mActionTrigger?.Invoke();
                        yield return new WaitForSeconds(GetComponent<ActionTrigger>().mTime);
                    }
                    else
                    {
                        mTime /= 9.0f;
                        yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime / 9.0f);
                        mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                    }
                    StartCoroutine(CounterState(mTarget.mStatus.mDamage));
                }
                yield return new WaitForSeconds(mTime);
                mAiBuild.actionEvent = ((mStatus.mHealth > 0.0f)) ? ActionEvent.BackWalk : ActionEvent.Busy;
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);

            }
            else if(mType == AttackType.Range)
            {
                mAiBuild.actionEvent = ActionEvent.Busy;
                PlayAnimation("Attack");
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length / 3.0f);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mSetting.Name),transform.Find("Fire").position,Quaternion.identity);
                Bullet bullet = go.GetComponent<Bullet>();
                bullet.Initialize(mTarget, mStatus.mDamage);
                yield return new WaitUntil(() => bullet.isDamaged == true);
            }
            else if(mType == AttackType.Instant)
            {
                mAiBuild.actionEvent = ActionEvent.Busy;
                PlayAnimation("Attack");
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime / 3.0f);
                Vector3 pos = new Vector3(mTarget.transform.position.x + 5.0f, mTarget.transform.position.y, mTarget.transform.position.z);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mSetting.Name), pos, Quaternion.identity);
                mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                Destroy(go, 1.0f);
                yield return new WaitUntil(() => go == null);
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
        yield return new WaitForSeconds(0.25f);
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
            yield return new WaitForSeconds(0.25f);
            TurnEnded();
        }
    }

    public IEnumerator CounterState(float dmg)
    {
        bool exist = false;
        for (int i = 0; i < mTarget.GetComponent<Animator>().parameters.Length; i++)
            if (mTarget.GetComponent<Animator>().parameters[i].name == "Melee")
                exist = true;
        if (exist)
            mTarget.GetComponent<Animator>().SetBool("Melee", false);
        if (mTarget.mBuffNerfController.SearchBuff("Counter"))
        {
            Counter counter = mTarget.mBuffNerfController.GetBuff("Counter") as Counter;
            if (counter.mChanceRate >= UnityEngine.Random.Range(0.0f, 1.0f))
            {
                if (exist)
                    mTarget.GetComponent<Animator>().SetBool("Melee", true);
                mTime += 0.25f;
                yield return new WaitForSeconds(0.25f);
                mTarget.mTarget = this;
                mTarget.mTarget.TakeDamage(dmg, DamageType.Magical);
                if (mStatus.mHealth <= 0.0f)
                {
                    mAiBuild.actionEvent = ActionEvent.Busy;
                    mGroundCheck.SetActive(false);
                    mTime = 0.1f;
                }
            }
        }
    }
    
    public bool DodgeState()
    {
        if (mBuffNerfController.SearchBuff("Dodge"))
        {
            Dodge dodge = mBuffNerfController.GetBuff("Dodge") as Dodge;
            return (dodge.mChanceRate >= UnityEngine.Random.Range(0.0f, 1.0f));
        }
        return false;
    }

    virtual public void PlayAnimation(string name)
    {
        mAnimator.Play(name);
    }

    virtual public void TakeDamage(float dmg, DamageType type)
    {
        if (mConditions.isDied)
            return;

        if (DodgeState())
            return;

        float value = dmg;
        if (type == DamageType.Physical)
        {
            value = (mConditions.isDefend) ? dmg - (dmg * mStatus.mDefend / 100.0f) : dmg;
            value = (value - (mStatus.mArmor + mBonusStatus.mArmor) <= 0.0f) ? 1.0f : value - (mStatus.mArmor + mBonusStatus.mArmor);
        }
        else
            value = (value - (mStatus.mMagic_Resistance + mBonusStatus.mMagic_Resistance) <= 0.0f) ? 1.0f : value - (mStatus.mMagic_Resistance + mBonusStatus.mMagic_Resistance);
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
                GameManager.s_TotalSoul += GameManager.Instance.mAmountofSoul;
            }
            mAnimator.SetBool("Death",true);
            mHealthBar.ActiveDeathAnimation(true);
            GetComponent<BoxCollider>().enabled = false;
            UIManager.Instance.mOrderbar.GetComponent<OrderBar>().DequeueOrder(this);
            mBuffNerfController.Stop();
            mField.GetComponent<Field>().Picked(false);
            mField.GetComponent<Field>().IsExist = false;
        }
        mAnimator.SetTrigger("Hit");
    }

    virtual public void TakeRecover(float val)
    {
        mStatus.mHealth += val;
        mHealthBar.mCurrentHealth = mStatus.mHealth;
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

    virtual public void SetBuff(TimedBuff buff)
    {
        mBuffNerfController.AddBuff(buff);
    }

    virtual public void SetNerf(TimedNerf nerf)
    {
        mBuffNerfController.AddNerf(nerf);
    }

    virtual public void ResetUnit()
    {
        Componenet_Initialize();
        Prefab_Initialize();
        AI_Initialize();
    }

    public void DisableUnit(Vector3 pos)
    {
        transform.position = pos;
        mHealthBar.Active(false);
        GetComponent<BuffAndNerfEntity>().Stop();
        gameObject.SetActive(false);
    }

    public void EnableUnit(int index)
    {
        if(mFlag == Flag.Player)
        {
            transform.position = BattleManager.Instance.playerCenter;
            mField = BattleManager.playerFieldParent.GetChild(index).gameObject;
            BattleManager.playerFieldParent.GetChild(index).GetComponent<Field>().IsExist = true;
        }
        else
        {
            transform.position = BattleManager.Instance.enemyCenter;
            mField = BattleManager.enemyFieldParent.GetChild(index).gameObject;
            BattleManager.enemyFieldParent.GetChild(index).GetComponent<Field>().IsExist = true;
        }

        mAiBuild.actionEvent = ActionEvent.IntroWalk;
        gameObject.SetActive(true);
    }


}
