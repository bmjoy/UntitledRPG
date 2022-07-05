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
    public Field mField;
    [HideInInspector]
    public GameObject mSelected = null;

    public AttackType mType = AttackType.Melee;
    public Flag mFlag;
    private GameObject mFirePos;
    public GameObject mCanvas;

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
    private float mAttackTime = 1.0f;

    [SerializeField]
    private float mDefendTime = 1.0f;

    [SerializeField]
    protected float mAttackDistance= 0.0f;

    [HideInInspector]
    public float mMagicDistance = 0.0f;

    public Action mActionTrigger = null;
    public Action mStartActionTrigger = null;
    private float mWalkTime = 0.0f;
    private float mMaxWalkTime = 0.3f;
    [HideInInspector]
    public float mCurrentSpeed = 1.0f;
    [HideInInspector]
    public Vector3 dodgePos = Vector3.zero;
    private Vector3 dodgeCurrentPos = Vector3.zero;
    [HideInInspector]
    public List<SoundClip> mRunClips = new List<SoundClip>();
    [HideInInspector]
    public List<SoundClip> mAttackClips = new List<SoundClip>();
    [HideInInspector]
    public List<SoundClip> mSkillClips = new List<SoundClip>();    
    [HideInInspector]
    public List<SoundClip> mDeathClips = new List<SoundClip>();
    public List<string> MyAttackAnim = new List<string>();
    public Mirror mirror;
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
        mConditions = new Conditions(false, false, false);

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

        MyAttackAnim.Clear();
        var animStates = mAnimator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < animStates.Length; i++)
        {
            if (animStates[i].name.Contains("Attack"))
                MyAttackAnim.Add(animStates[i].name);
        }
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
            + new Vector3(0.0f,GetComponent<BoxCollider>().center.y + 0.5f, 0.0f), Quaternion.identity, transform);
        mCanvas.GetComponent<Canvas>().sortingOrder = 8;
        mSelected = mCanvas.transform.Find("Selected").gameObject;
        mHealthBar = mCanvas.transform.Find("Borader").Find("HealthBarPrefab").GetComponent<MiniHealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth, mStatus.mMana, mStatus.mMaxMana);
        mLevelText = mCanvas.transform.Find("Borader").Find("Text").GetComponent<TextMeshProUGUI>();
        mLevelText.text = mStatus.mLevel.ToString();
        mLevelText.gameObject.SetActive(true);
        mHealthBar.Active(false);
        mSelected.SetActive(false);

        mRunClips.Clear();
        mAttackClips.Clear();
        if(mSetting.Clips.Count > 0)
        {
            mRunClips = mSetting.Clips.FindAll(s => s.Type == SoundClip.SoundType.Run);
            mAttackClips = mSetting.Clips.FindAll(s => s.Type == SoundClip.SoundType.Attack);
            mSkillClips = mSetting.Clips.FindAll(s => s.Type == SoundClip.SoundType.Skill);
        }
    }

    public void AI_Initialize()
    {
        mAiBuild.SetActionEvent(ActionEvent.IntroWalk);
        if (isAIinitialized)
            return;
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
        if (mConditions.isDied == false)
            mLevelText.gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        if (CameraSwitcher.isCollided)
            GetComponent<SpriteRenderer>().flipX = (mFlag == Flag.Player) ? true : false;
        mHealthBar.mCurrentMana = mStatus.mMana + mBonusStatus.mMana;
        mHealthBar.mCurrentHealth = mStatus.mHealth + mBonusStatus.mHealth;
        mHealthBar.mMaxHealth = mStatus.mMaxHealth + mBonusStatus.mHealth;
        mHealthBar.mMaxMana = mStatus.mMaxMana + mBonusStatus.mMana;

        switch (mAiBuild.actionEvent)
        {
            case ActionEvent.None:
                {
                    mirror?.SetFloat("Speed", 0.0f);
                    mirror?.SetBool("Death", (mConditions.isDied) ? true : false);
                    mAnimator.SetFloat("Speed", 0.0f);
                    mAnimator.SetBool("Death", (mConditions.isDied) ? true : false);
                    mAiBuild.Update((mAiBuild.type == AIType.Auto));
                    dodgeCurrentPos = transform.position;
                    dodgePos = (mFlag == Flag.Player) ? transform.position - new Vector3(0.0f,0.0f, 2.5f)
            : transform.position + new Vector3(0.0f, 0.0f, 2.5f);
                }
                break;
            case ActionEvent.IntroWalk:
                {
                    mAiBuild.SetActionEvent(Run(mField.transform.position, 0.1f, ActionEvent.None, ActionEvent.IntroWalk));
                    mHealthBar.Active((mAiBuild.actionEvent == ActionEvent.None));
                }
                break;
            case ActionEvent.AttackWalk:
                mAiBuild.SetActionEvent(Run(mTarget.transform.position, mAttackDistance, ActionEvent.Busy, ActionEvent.AttackWalk));
                break;
            case ActionEvent.MagicWalk:
                mAiBuild.SetActionEvent(Run(mTarget.transform.position, mMagicDistance, ActionEvent.Busy, ActionEvent.MagicWalk));
                break;
            case ActionEvent.BackWalk:
                mAiBuild.SetActionEvent(Run(mField.transform.position, 0.1f, ActionEvent.Busy, ActionEvent.BackWalk));
                break;
            case ActionEvent.Busy:
                {
                    mirror?.SetFloat("Speed", 0.0f);
                    mAnimator.SetFloat("Speed", 0.0f);
                    dodgeCurrentPos = transform.position;
                    dodgePos = (mFlag == Flag.Player) ? transform.position - new Vector3(0.0f, 0.0f, 2.5f)
: transform.position + new Vector3(0.0f, 0.0f, 2.5f);
                }
                break;
            case ActionEvent.DodgeWalk:
                mAiBuild.SetActionEvent((mConditions.isDied) ? ActionEvent.None : Run(dodgePos, 0.1f, ActionEvent.DodgeBack, ActionEvent.DodgeWalk));
                break;
            case ActionEvent.DodgeBack:
                mAiBuild.SetActionEvent((mConditions.isDied) ? ActionEvent.None : Run(dodgeCurrentPos, 0.1f, ActionEvent.None, ActionEvent.DodgeBack));
                break;
        }
        CheckGround();
        if(mAnimator.GetFloat("Speed") > 0.1f && mRunClips.Count > 0)
        {
            mWalkTime += Time.deltaTime;
            if(mWalkTime >= mMaxWalkTime)
            {
                AudioManager.PlaySfx(mRunClips[Random.Range(0, mRunClips.Count - 1)].Clip, 0.6f);
                mWalkTime = 0.0f;
            }
        }
    }

    protected ActionEvent Run(Vector3 to, float maxDist, ActionEvent actionEvent1, ActionEvent actionEvent2)
    {
        transform.position = Vector3.MoveTowards(transform.position, to, Time.deltaTime * BattleManager.Instance.mRunningSpeed);
        mAnimator.SetFloat("Speed", (mCurrentSpeed < 4.9f) ? 1.0f : mCurrentSpeed);
        mirror?.SetFloat("Speed", (mCurrentSpeed < 4.9f) ? 1.0f : mCurrentSpeed);
        return ((Vector3.Distance(transform.position, to) < maxDist)) ? actionEvent1 : actionEvent2;
    }

    protected void CheckGround()
    {
        isGrounded = Physics.CheckSphere(mGroundCheck.transform.position, mGroundDistance, LayerMask.GetMask("Ground"));
        mVelocity.y = (isGrounded && mVelocity.y <= 0.0f) ? -GetComponent<BoxCollider>().size.y + 0.2f : mVelocity.y;
        mVelocity.y += -9.8f * Time.deltaTime;
        mRigidbody.AddForce(mVelocity * Time.deltaTime);
        if (transform.position.y <= -50.0f)
            transform.position = mField.transform.position + new Vector3(0.0f,1.0f,0.0f);
    }
    virtual public KeyValuePair<bool, BonusStatus> LevelUP()
    {
        BonusStatus bonus = new BonusStatus();
        if (mStatus.mEXP >= GameManager.Instance.mRequiredEXP + (50.0f * mStatus.mLevel))
        {
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mLevelUPSFX);
            bonus.mHealth = Random.Range(5, 10);
            bonus.mMana = Random.Range(5, 10);
            bonus.mDamage = Random.Range(1, 5);
            bonus.mMagicPower = Random.Range(1, 5);
            bonus.mArmor = Random.Range(1, 5);
            bonus.mMagic_Resistance = Random.Range(1, 5);
            mStatus.mEXP = mStatus.mEXP - (GameManager.Instance.mRequiredEXP + (50 * mStatus.mLevel));
            mStatus.mLevel++;
            mStatus.mMaxHealth += bonus.mHealth;
            mStatus.mMaxMana += bonus.mMana;
            mStatus.mDamage += bonus.mDamage;
            mStatus.mMagicPower += bonus.mMagicPower;
            mStatus.mArmor += bonus.mArmor;
            mStatus.mMagic_Resistance += bonus.mMagic_Resistance;
            mLevelText.text = mStatus.mLevel.ToString();
            
            return new KeyValuePair<bool, BonusStatus>(true, bonus);
        }
        return new KeyValuePair<bool, BonusStatus>(false, bonus);
    }
    virtual public IEnumerator AttackAction(DamageType type, Action onComplete)
    {
        mConditions.isCancel = false;
        if (mAiBuild.type == AIType.Manual)
        {
            UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForTarget);
            foreach (GameObject enemy in BattleManager.Instance.mEnemies)
            {
                if (!enemy.GetComponent<Unit>().mConditions.isDied)
                    enemy.GetComponent<Unit>().mCanvas.transform.Find("Arrow").gameObject.SetActive(true);
            }
            float maxDist = 0.0f;
            while (true)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, (mFlag == Flag.Player) ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Ally")))
                {
                    if(maxDist < hit.distance)
                    {
                        mTarget?.mField.TargetedHostile(false);
                        mTarget?.mSelected.SetActive(false);
                        mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                        mTarget?.mSelected.SetActive(true);
                        maxDist = hit.distance;
                    }

                    if(mTarget && mTarget.gameObject != hit.collider.gameObject)
                    {
                        mTarget?.mField.TargetedHostile(false);
                        mTarget?.mSelected.SetActive(false);
                        mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
                        mTarget?.mSelected.SetActive(true);
                        maxDist = hit.distance;
                    }

                    mTarget?.mField.TargetedHostile(true);

                    if (mTarget && Input.GetMouseButtonDown(0)) break;
                }
                else
                {
                    maxDist = 0.0f;
                    mTarget?.mField.TargetedHostile(false);
                    mTarget?.mSelected.SetActive(false);
                    mTarget = null;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    maxDist = 0.0f;
                    BattleManager.Instance.Cancel();
                    UIManager.Instance.ChangeOrderBarText("Waiting for Order...");
                    break;
                }
                yield return null;
            }
            foreach (GameObject enemy in BattleManager.Instance.mEnemies)
                enemy.GetComponent<Unit>().mCanvas.transform.Find("Arrow").gameObject.SetActive(false);
        }
        else
            yield return new WaitForSeconds(0.4f);

        if (mConditions.isCancel == false && mTarget)
        {
            mTarget.mField.TargetedHostile(false);
            UIManager.Instance.ChangeOrderBarText("Battle Start!");
            mTarget.mSelected.SetActive(false);
            mSpriteRenderer.sortingOrder = (transform.position.z < mTarget?.transform.position.z) ? 3 : 4;
            mTarget.mSpriteRenderer.sortingOrder = (transform.position.z > mTarget?.transform.position.z) ? 3 : 4;

            onComplete?.Invoke();
            if (mType == AttackType.Melee)
            {
                mAiBuild.SetActionEvent(ActionEvent.AttackWalk);
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
                mirror?.Play("Attack");
                if (mTarget)
                {
                    if (mActionTrigger != null)
                    {
                        mActionTrigger?.Invoke();
                        yield return new WaitUntil(()=> GetComponent<ActionTrigger>().isCompleted);
                    }
                    else
                    {
                        mAnimator.Play(MyAttackAnim[Random.Range(0, MyAttackAnim.Count)]);
                        if (mAttackClips.Count > 0)
                            AudioManager.PlaySfx(mAttackClips[Random.Range(0, mAttackClips.Count)].Clip, 0.6f);
                        yield return new WaitForSeconds(mAttackTime);

                        mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                    }
                    StartCoroutine(CounterState(mTarget.mStatus.mDamage));
                }
                yield return new WaitForSeconds(1.0f);
                mAiBuild.SetActionEvent(((mStatus.mHealth > 0.0f)) ? ActionEvent.BackWalk : ActionEvent.Busy);
                TurnEnded();
                yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);

            }
            else if(mType == AttackType.Range)
            {
                mAiBuild.SetActionEvent(ActionEvent.Busy);
                mAnimator.Play("Attack");
                mirror?.Play("Attack");
                if (mAttackClips.Count > 0)
                    AudioManager.PlaySfx(mAttackClips[Random.Range(0, mAttackClips.Count)].Clip, 0.6f);
                yield return new WaitForSeconds(mAttackTime);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mSetting.Name),transform.Find("Fire").position,Quaternion.identity);
                if(go.GetComponent<SpriteRenderer>())
                    go.GetComponent<SpriteRenderer>().flipX = transform.GetComponent<SpriteRenderer>().flipX;
                Bullet bullet = go.GetComponent<Bullet>();
                bullet.Initialize(mTarget.transform, mStatus.mDamage + mBonusStatus.mDamage);

                yield return new WaitUntil(() => bullet.isDamaged == true);
                TurnEnded();
            }
            else if(mType == AttackType.Instant)
            {
                mirror?.Play("Attack");
                mAiBuild.SetActionEvent(ActionEvent.Busy);
                mAnimator.Play("Attack");
                if (mAttackClips.Count > 0)
                    AudioManager.PlaySfx(mAttackClips[Random.Range(0, mAttackClips.Count)].Clip, 0.6f);
                yield return new WaitForSeconds(mAttackTime);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Bullets/" + mSetting.Name), mTarget.transform.position + new Vector3(5.0f, 0.0f, 0.0f), Quaternion.identity);
                mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                Destroy(go, 1.0f);

                yield return new WaitUntil(() => go == null);
                TurnEnded();

            }
            mAiBuild.SetActionEvent(ActionEvent.None);
            mAiBuild.ChangeState("Waiting");
        }
    }

    virtual public IEnumerator DefendAction(Action onComplete)
    {
        mConditions.isDefend = true;
        onComplete?.Invoke();
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDefendSFX);
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Defend"),
            new Vector3(transform.position.x,
            transform.position.y + GetComponent<BoxCollider>().size.y / 2.0f,
            transform.position.z), Quaternion.identity);
        Destroy(go, 1.5f);
        yield return new WaitForSeconds(mDefendTime);
        mAiBuild.ChangeState("Waiting");
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
            TurnEnded();
            yield return new WaitForSeconds(1.0f);
            mAiBuild.SetActionEvent(ActionEvent.None);
            mAiBuild.ChangeState("Waiting");
        }
    }

    public IEnumerator CounterState(float dmg)
    {
        if (mTarget.mBuffNerfController.SearchBuff("Counter"))
        {
            Counter counter = mTarget.mBuffNerfController.GetBuff("Counter") as Counter;
            if (counter.mChanceRate >= UnityEngine.Random.Range(0.0f, 1.0f))
            {
                yield return new WaitForSeconds(0.25f);
                if (mTarget.mAttackClips.Count > 0)
                    AudioManager.PlaySfx(mTarget.mAttackClips[Random.Range(0, mTarget.mAttackClips.Count)].Clip, 0.6f);
                mTarget.mTarget = this;
                if(mTarget.MyAttackAnim.Count > 0)
                    mTarget.mAnimator.Play(mTarget.MyAttackAnim[Random.Range(0,mTarget.MyAttackAnim.Count)]);
                mTarget.mTarget.TakeDamage(dmg, DamageType.Physical);
                if (mStatus.mHealth <= 0.0f)
                {
                    mAiBuild.actionEvent = ActionEvent.Busy;
                    mGroundCheck.SetActive(false);
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
        else
        {
            float myChanceRate = (float)Math.Round(Mathf.Sqrt(mStatus.mAgility + mBonusStatus.mAgility) / 100, 2);
            float random = UnityEngine.Random.Range(0.00f, 1.00f);
            return myChanceRate >= random;
        }
    }

    virtual public bool TakeDamage(float dmg, DamageType type)
    {
        bool isDodge = DodgeState();
        if (mConditions.isDied) return false;

        float value = dmg;
        if (type == DamageType.Physical)
        {
            value = (mConditions.isDefend) ? dmg - (dmg * mStatus.mDefend / 100.0f) : dmg;
            value = (value - (mStatus.mArmor + mBonusStatus.mArmor) <= 0.0f) ? 1.0f : value - (mStatus.mArmor + mBonusStatus.mArmor);
        }
        else
        {
            value = (value - (mStatus.mMagic_Resistance + mBonusStatus.mMagic_Resistance) <= 0.0f) ? 1.0f : value - (mStatus.mMagic_Resistance + mBonusStatus.mMagic_Resistance);
            isDodge = false;
        }
        if (isDodge)
        {
            mAiBuild.actionEvent = ActionEvent.DodgeWalk;
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDodgeSFX);
        }
        GameObject damage = Instantiate(mCanvas.transform.Find("DamageValue").gameObject
            , mCanvas.transform.position - new Vector3(0.0f,1.0f,0.0f), Quaternion.identity, mCanvas.transform);
        damage.SetActive(true);
        damage.GetComponent<TextMeshProUGUI>().text = (isDodge) ? "Miss!" : value.ToString();
        Destroy(damage, 1.1f);
        if(!isDodge)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>
                ("Prefabs/Effects/Hit"), transform.position + new Vector3(Random.Range(-1.0f,1.0f), Random.Range(0.5f, 1.2f), Random.Range(-1.0f, 1.0f)), Quaternion.identity,transform);
            go.GetComponent<Renderer>().sortingLayerName = "Foreground";
            Destroy(go, 1.1f);
            mStatus.mHealth -= value;
            mHealthBar.mCurrentHealth = mStatus.mHealth + mBonusStatus.mHealth;
            mHealthBar.StartCoroutine(mHealthBar.PlayBleed());
            if (mStatus.mHealth <= 0.0f)
            {
                if (mDeathClips.Count > 0)
                    AudioManager.PlaySfx(mDeathClips[Random.Range(0, mDeathClips.Count - 1)].Clip, 0.6f);

                mSelected.SetActive(false);
                mConditions.isDied = true;
                mLevelText.gameObject.SetActive(false);
                mStatus.mHealth = 0.0f;
                mHealthBar.mCurrentHealth = 0.0f;
                if (mFlag == Flag.Enemy)
                {
                    GameManager.s_TotalExp += mStatus.mEXP;
                    GameManager.s_TotalGold += mStatus.mGold;
                    GameManager.s_TotalSoul += GameManager.Instance.mAmountofSoul;
                }
                mirror?.SetBool("Death", true);
                mAnimator.SetBool("Death", true);
                mHealthBar.ActiveDeathAnimation(true);
                GetComponent<BoxCollider>().enabled = mField.GetComponent<Field>().IsExist = false;
                UIManager.Instance.mStorage.mOrderbar.GetComponent<OrderBar>().DequeueOrder(this);
                mBuffNerfController.Stop();
                mField.GetComponent<Field>().Picked(false);
            }
            mirror?.SetTrigger("Hit");
            mAnimator.SetTrigger("Hit");
        }

        return !isDodge;
    }

    virtual public void TakeDamageByTrap(float dmg)
    {
        if (mConditions.isDied) return;
        mStatus.mHealth -= dmg;
        if (mStatus.mHealth <= 0.0f)
        {
            mConditions.isDied = true;
            mStatus.mHealth = 0.0f;
            mHealthBar.mCurrentHealth = 0.0f;
        }
    }
    virtual public void TakeRecover(float val)
    {
        mStatus.mHealth += val;
        if (mStatus.mHealth >= mStatus.mMaxHealth + mBonusStatus.mHealth)
            mStatus.mHealth = mStatus.mMaxHealth + mBonusStatus.mHealth;
        mHealthBar.mCurrentHealth = mStatus.mHealth + mBonusStatus.mHealth;
    }

    virtual public void TakeRecoverMana(float val)
    {
        mStatus.mMana += val;
        if (mStatus.mMana >= mStatus.mMaxMana + mBonusStatus.mMana)
            mStatus.mMana = mStatus.mMaxMana + mBonusStatus.mMana;
        mHealthBar.mCurrentMana = mStatus.mMana + mBonusStatus.mMana;
    }
    virtual public void TurnEnded()
    {
        if(mTarget != null)
        {
            mTarget.mSelected.SetActive(false);
            mSpriteRenderer.sortingOrder = mTarget.mSpriteRenderer.sortingOrder = 4;
        }
        mField.GetComponent<Field>().Picked(false);

        mTarget = null;
        mOrder = Order.TurnEnd;
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

        mStatus.mMaxHealth -= mBonusStatus.mHealth;
        if (mStatus.mHealth > mStatus.mMaxHealth + mBonusStatus.mHealth)
            mStatus.mHealth = mStatus.mMaxHealth + mBonusStatus.mHealth;
        mStatus.mMaxMana -= mBonusStatus.mMana;
        if (mStatus.mMana > mStatus.mMaxMana + mBonusStatus.mMana)
            mStatus.mMana = mStatus.mMaxMana + mBonusStatus.mMana;
        mBuffNerfController.Stop();
        mAiBuild.SetActionEvent(ActionEvent.None);
        gameObject.SetActive(false);
    }

    public void EnableUnit(int index)
    {
        if(mFlag == Flag.Player)
        {
            transform.position = BattleManager.Instance.playerCenter;
            mField = BattleManager.playerFieldParent.GetChild(index).GetComponent<Field>();
            BattleManager.playerFieldParent.GetChild(index).GetComponent<Field>().IsExist = true;
        }
        else
        {
            transform.position = BattleManager.Instance.enemyCenter;
            mField = BattleManager.enemyFieldParent.GetChild(index).GetComponent<Field>();
            BattleManager.enemyFieldParent.GetChild(index).GetComponent<Field>().IsExist = true;
        }
        mStatus.mHealth += mBonusStatus.mHealth;
        mStatus.mMaxHealth += mBonusStatus.mHealth;

        mStatus.mMana += mBonusStatus.mMana;
        mStatus.mMaxMana += mBonusStatus.mMana;
        
        mAiBuild.SetActionEvent(ActionEvent.IntroWalk);

        gameObject.SetActive(true);
        if (mConditions.isDied == true)
        {
            mHealthBar?.ActiveDeathAnimation(true);
            mLevelText.gameObject.SetActive(false);
            mAnimator.SetBool("Death", true);
        }
        mirror = null;
    }

    public void Revive(float val)
    {
        mAnimator.SetBool("Death", false);
        mHealthBar.ActiveDeathAnimation(false);
        mLevelText.gameObject.SetActive(true);
        mConditions.isDied = false;
        GetComponent<BoxCollider>().enabled = mField.GetComponent<Field>().IsExist = true;
        TakeRecover(Mathf.RoundToInt((val * mStatus.mMaxHealth) / (mStatus.mMaxHealth + mBonusStatus.mHealth)));
        TakeRecoverMana(Mathf.RoundToInt((val * mStatus.mMaxMana) / (mStatus.mMaxMana + mBonusStatus.mMana)));
        UIManager.Instance.mStorage.mOrderbar.GetComponent<OrderBar>().EnqueueSignleOrder(this);
    }
}
