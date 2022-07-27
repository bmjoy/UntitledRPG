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
    [HideInInspector]
    public GameObject mCanvas;
    [HideInInspector]
    public GameObject mArrow;

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
  
    public AIBuild mAiBuild;
    public Status mStatus;
    public BonusStatus mBonusStatus;
    public Conditions mConditions;

    private Vector3 mVelocity = Vector3.zero;
    private GameObject mGroundCheck;
    private float mGroundDistance = 2.0f;

    [SerializeField]
    public float mAttackTime = 1.0f;

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
    protected string mProjectileName = string.Empty;
    [HideInInspector]
    public float mCurrentSpeed = 1.0f;
    [HideInInspector]
    public Vector3 dodgePos = Vector3.zero;
    private Vector3 dodgeCurrentPos = Vector3.zero;
    [HideInInspector]
    public IEnumerable<SoundClip> mRunClips = Enumerable.Empty<SoundClip>();
    [HideInInspector]
    public IEnumerable<SoundClip> mAttackClips = Enumerable.Empty<SoundClip>();
    [HideInInspector]
    public IEnumerable<SoundClip> mSkillClips = Enumerable.Empty<SoundClip>();    
    [HideInInspector]
    public IEnumerable<SoundClip> mDeathClips = Enumerable.Empty<SoundClip>();
    [HideInInspector]
    public List<string> MyAttackAnim = new List<string>();
    [HideInInspector]
    public Mirror mirror;
    protected virtual void Start()
    {

    }

    public void Componenet_Initialize()
    {
        mStatus = new Status(mSetting);
        mConditions = new Conditions(false);
        mBonusStatus = new BonusStatus(true);
        mAnimator = GetComponent<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
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
            GameObject groundCheck = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UnitGroundCheck"),(
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
        mCanvas = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/CanvasForUnit"), transform.position
            + new Vector3(0.0f,GetComponent<BoxCollider>().center.y + 0.5f, 0.0f), Quaternion.identity, transform);
        mCanvas.GetComponent<Canvas>().sortingOrder = 8;
        mArrow = mCanvas.transform.Find("Arrow").gameObject;
        mSelected = mCanvas.transform.Find("Selected").gameObject;
        mHealthBar = mCanvas.transform.Find("Borader").Find("HealthBarPrefab").GetComponent<MiniHealthBar>();
        mHealthBar.Initialize(mStatus.mHealth, mStatus.mMaxHealth, mStatus.mMana, mStatus.mMaxMana);
        mLevelText = mCanvas.transform.Find("Borader").Find("Text").GetComponent<TextMeshProUGUI>();
        mLevelText.text = mStatus.mLevel.ToString();
        mLevelText.gameObject.SetActive(true);
        mHealthBar.Active(false);
        mSelected.SetActive(false);
        mProjectileName = mSetting.Name;
        if(mSetting.Clips.Count > 0)
        {
            mRunClips = mSetting.Clips.FindAll(s => s.Type == SoundClip.SoundType.Run);
            mAttackClips = mSetting.Clips.FindAll(s => s.Type == SoundClip.SoundType.Attack);
            mSkillClips = mSetting.Clips.FindAll(s => s.Type == SoundClip.SoundType.Skill);
        }
    }

    public void AI_Initialize()
    {
        mAiBuild = new AIBuild(AIBuild.AIType.Auto ,true);
        if(GetComponent<StateMachine>() == null)
        {
            mAiBuild.stateMachine = gameObject.AddComponent<StateMachine>();
            mAiBuild.stateMachine.mAgent = this;
            mAiBuild.SetBasicStates();
        }
        else
        {
            mAiBuild.stateMachine = GetComponent<StateMachine>();
            mAiBuild.stateMachine.mAgent = this;
        }

    }

    protected virtual void Update()
    {
        if (CameraSwitcher.isCollided)
            GetComponent<SpriteRenderer>().flipX = (mFlag == Flag.Player) ? true : false;
        mHealthBar.mCurrentMana = mStatus.mMana;
        mHealthBar.mCurrentHealth = mStatus.mHealth;
        mHealthBar.mMaxHealth = mStatus.mMaxHealth;
        mHealthBar.mMaxMana = mStatus.mMaxMana;

        if (mAnimator.GetFloat("Speed") > 0.1f && (mRunClips.Count() > 0))
        {
            mWalkTime += Time.deltaTime;
            if (mWalkTime >= mMaxWalkTime)
            {
                AudioManager.PlaySfx(mRunClips.ElementAt(Random.Range(0, mRunClips.Count())).Clip, 0.6f);
                mWalkTime = 0.0f;
            }
        }
    }

    private void FixedUpdate()
    {
        if (mConditions.isDied == false)
            mLevelText.gameObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);

        switch (mAiBuild.actionEvent)
        {
            case AIBuild.ActionEvent.None:
                {
                    mirror?.SetFloat("Speed", 0.0f);
                    mirror?.SetBool("Death", (mConditions.isDied) ? true : false);
                    mAnimator.SetFloat("Speed", 0.0f);
                    mAnimator.SetBool("Death", (mConditions.isDied) ? true : false);
                    mAiBuild.Update((mAiBuild.type == AIBuild.AIType.Auto));
                    dodgeCurrentPos = transform.position;
                    dodgePos = (mFlag == Flag.Player) ? transform.position - new Vector3(0.0f, 0.0f, 2.5f)
            : transform.position + new Vector3(0.0f, 0.0f, 2.5f);
                }
                break;
            case AIBuild.ActionEvent.IntroWalk:
                {
                    mAiBuild.SetActionEvent(Run(mField.transform.position, 0.1f, AIBuild.ActionEvent.None, AIBuild.ActionEvent.IntroWalk));
                    mHealthBar.Active((mAiBuild.actionEvent == AIBuild.ActionEvent.None));
                }
                break;
            case AIBuild.ActionEvent.AttackWalk:
                mAiBuild.SetActionEvent(Run(mTarget.transform.position, mAttackDistance, AIBuild.ActionEvent.Busy, AIBuild.ActionEvent.AttackWalk));
                break;
            case AIBuild.ActionEvent.MagicWalk:
                mAiBuild.SetActionEvent(Run(mTarget.transform.position, mMagicDistance, AIBuild.ActionEvent.Busy, AIBuild.ActionEvent.MagicWalk));
                break;
            case AIBuild.ActionEvent.BackWalk:
                mAiBuild.SetActionEvent(Run(mField.transform.position, 0.1f, AIBuild.ActionEvent.Busy, AIBuild.ActionEvent.BackWalk));
                break;
            case AIBuild.ActionEvent.Busy:
                {
                    mirror?.SetFloat("Speed", 0.0f);
                    mAnimator.SetFloat("Speed", 0.0f);
                    dodgeCurrentPos = transform.position;
                    dodgePos = (mFlag == Flag.Player) ? transform.position - new Vector3(0.0f, 0.0f, 2.5f)
: transform.position + new Vector3(0.0f, 0.0f, 2.5f);
                }
                break;
            case AIBuild.ActionEvent.DodgeWalk:
                mAiBuild.SetActionEvent((mConditions.isDied) ? AIBuild.ActionEvent.None : Run(dodgePos, 0.1f, AIBuild.ActionEvent.DodgeBack, AIBuild.ActionEvent.DodgeWalk));
                break;
            case AIBuild.ActionEvent.DodgeBack:
                mAiBuild.SetActionEvent((mConditions.isDied) ? AIBuild.ActionEvent.None : Run(dodgeCurrentPos, 0.1f, AIBuild.ActionEvent.None, AIBuild.ActionEvent.DodgeBack));
                break;
        }

        CheckGround();
    }

    protected AIBuild.ActionEvent Run(Vector3 to, float maxDist, AIBuild.ActionEvent actionEvent1, AIBuild.ActionEvent actionEvent2)
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
        mRigidbody.velocity = mVelocity * Time.deltaTime;
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

            if(mConditions.isDied == false)
            {
                mStatus.mHealth = mStatus.mMaxHealth;
                mStatus.mMana = mStatus.mMaxMana;
            }
            return new KeyValuePair<bool, BonusStatus>(true, bonus);
        }
        return new KeyValuePair<bool, BonusStatus>(false, bonus);
    }
    virtual public IEnumerator AttackAction(DamageType type, Action onComplete)
    {
        mConditions.isCancel = false;
        if (mAiBuild.type == AIBuild.AIType.Manual)
        {
            UIManager.Instance.ChangeOrderBarText(UIManager.Instance.mStorage.mTextForTarget);
            DisplayArrow(true);
            Field enemy = null;
            mTarget = null;
            enemy = RandomTargeting(ref enemy);
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    enemy.TargetedHostile(false);
                    mTarget = null;
                    enemy = BattleManager.enemyFieldParent.GetChild(0).GetComponent<Field>();
                    if (enemy.IsExist)
                    {
                        mTarget = enemy.mUnit;
                        enemy.TargetedHostile(true);
                    }
                    else
                        enemy = RandomTargeting(ref enemy);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    enemy.TargetedHostile(false);
                    mTarget = null;
                    enemy = BattleManager.enemyFieldParent.GetChild(1).GetComponent<Field>();
                    if (enemy.IsExist)
                    {
                        mTarget = enemy.mUnit;
                        enemy.TargetedHostile(true);
                    }
                    else
                        enemy = RandomTargeting(ref enemy);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    enemy.TargetedHostile(false);
                    mTarget = null;
                    enemy = BattleManager.enemyFieldParent.GetChild(2).GetComponent<Field>();
                    if (enemy.IsExist)
                    {
                        mTarget = enemy.mUnit;
                        enemy.TargetedHostile(true);
                    }
                    else
                        enemy = RandomTargeting(ref enemy);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    enemy.TargetedHostile(false);
                    mTarget = null;
                    enemy = BattleManager.enemyFieldParent.GetChild(3).GetComponent<Field>();
                    if (enemy.IsExist)
                    {
                        mTarget = enemy.mUnit;
                        enemy.TargetedHostile(true);
                    }
                    else
                        enemy = RandomTargeting(ref enemy);

                }
                else {}

                if (Input.GetKeyDown(UIManager.Instance.mYesKeyCode) && mTarget)
                    break;
                if (Input.GetKeyDown(UIManager.Instance.mNoKeyCode))
                {
                    enemy.TargetedHostile(false);
                    enemy.TargetedFriendly(false);
                    enemy = null;
                    mTarget = null;
                    BattleManager.Instance.Cancel();
                    UIManager.Instance.ChangeOrderBarText("Waiting for Order...");
                    break;
                }
                yield return null;
            }
            DisplayArrow(false);
        }
        else
            yield return new WaitForSeconds(0.4f);

        if (mConditions.isCancel == false && mTarget)
        {
            mTarget.mField.TargetedHostile(false);
            UIManager.Instance.ChangeOrderBarText("Battle Start!");
            mTarget.mSelected.SetActive(false);

            onComplete?.Invoke();
            StartCoroutine(BattleState(type));
            yield return new WaitUntil(() => mConditions.isBattleComplete == true);
            mAiBuild.SetActionEvent(AIBuild.ActionEvent.None);
            mAiBuild.ChangeState("Waiting");
        }
    }

    private static void DisplayArrow(bool display)
    {
        for (int i = 0; i < BattleManager.Instance.mEnemies.Count; ++i)
        {
            Unit e = BattleManager.Instance.mEnemies[i].GetComponent<Unit>();
            if(!e.mConditions.isDied)
                e.mArrow.SetActive(display);
        }
    }

    private Field RandomTargeting(ref Field enemy)
    {
        for (int i = 0; i < BattleManager.enemyFieldParent.childCount; ++i)
        {
            enemy = BattleManager.enemyFieldParent.GetChild(i).GetComponent<Field>();
            if (enemy.IsExist)
            {
                mTarget = enemy.mUnit;
                enemy.TargetedHostile(true);
                break;
            }
        }

        return enemy;
    }

    virtual protected IEnumerator BattleState(DamageType type)
    {
        if (mType == AttackType.Melee)
        {
            mAiBuild.SetActionEvent(AIBuild.ActionEvent.AttackWalk);
            yield return new WaitUntil(() => mAiBuild.actionEvent == AIBuild.ActionEvent.Busy);
            mirror?.Play("Attack");
            if (mTarget)
            {
                if (mActionTrigger != null)
                {
                    mActionTrigger?.Invoke();
                    yield return new WaitUntil(() => GetComponent<ActionTrigger>().isCompleted);
                }
                else
                {
                    mAnimator.Play(MyAttackAnim[Random.Range(0, MyAttackAnim.Count)]);
                    if (mAttackClips.Count() > 0)
                        AudioManager.PlaySfx(mAttackClips.ElementAt(Random.Range(0, mAttackClips.Count())).Clip, 0.6f);
                    yield return new WaitForSeconds(mAttackTime);

                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                }
                StartCoroutine(CounterState(mTarget.mStatus.mDamage));
            }
            yield return new WaitForSeconds(1.0f);
            mAiBuild.SetActionEvent(((mStatus.mHealth > 0.0f)) ? AIBuild.ActionEvent.BackWalk : AIBuild.ActionEvent.Busy);
            yield return new WaitUntil(() => mAiBuild.actionEvent == AIBuild.ActionEvent.Busy);
        }
        else if (mType == AttackType.Range)
        {
            mAiBuild.SetActionEvent(AIBuild.ActionEvent.Busy);
            mAnimator.Play("Attack");
            mirror?.Play("Attack");
            if (mAttackClips.Count() > 0)
                AudioManager.PlaySfx(mAttackClips.ElementAt(Random.Range(0, mAttackClips.Count())).Clip, 0.6f);
            yield return new WaitForSeconds(mAttackTime);
            Bullet go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Bullets/" + mProjectileName), transform.Find("Fire").position, Quaternion.identity).GetComponent<Bullet>();
            go.Initialize(mTarget.transform, mStatus.mDamage + mBonusStatus.mDamage);
            yield return new WaitUntil(() => go.isDamaged == true);
        }
        else if (mType == AttackType.Instant)
        {
            mAiBuild.SetActionEvent(AIBuild.ActionEvent.Busy);
            mirror?.Play("Attack");
            mAnimator.Play("Attack");
            if (mAttackClips.Count() > 0)
                AudioManager.PlaySfx(mAttackClips.ElementAt(Random.Range(0, mAttackClips.Count())).Clip, 0.6f);
            yield return new WaitForSeconds(mAttackTime);
            GameObject go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Bullets/" + mProjectileName), mTarget.transform.position + new Vector3(5.0f, 0.0f, 0.0f), Quaternion.identity);
            mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
            Destroy(go, 1.0f);
            yield return new WaitUntil(() => go == null);
        }
        TurnEnded();
    }

    private void RayCast(float maxDist)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, (mFlag == Flag.Player) ? LayerMask.GetMask("Enemy") : LayerMask.GetMask("Ally")))
        {
            if ((maxDist < hit.distance) || (mTarget && mTarget.gameObject != hit.collider.gameObject))
                maxDist = SetTarget(ref hit);
            mTarget?.mField.TargetedHostile(true);
        }
        else
        {
            maxDist = 0.0f;
            mTarget?.mField.TargetedHostile(false);
            mTarget?.mSelected.SetActive(false);
            mTarget = null;
        }

    }
    private float SetTarget(ref RaycastHit hit)
    {
        float maxDist;
        mTarget?.mField.TargetedHostile(false);
        mTarget?.mSelected.SetActive(false);
        mTarget = (hit.transform.GetComponent<Unit>().mConditions.isDied == false) ? hit.transform.GetComponent<Unit>() : null;
        mTarget?.mSelected.SetActive(true);
        maxDist = hit.distance;
        return maxDist;
    }

    virtual public IEnumerator DefendAction(Action onComplete)
    {
        mConditions.isDefend = true;
        onComplete?.Invoke();
        AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDefendSFX);
        GameObject go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/Defend"),
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
        yield return new WaitForSeconds(0.1f);
        mSkillDataBase.Use();
        onComplete?.Invoke();
        yield return new WaitUntil(() => mSkillDataBase.Skill.isComplete == true);
        if (mSkillDataBase.Skill.isActive == false)
            BattleManager.Instance.Cancel();
        else
        {
            TurnEnded();
            yield return new WaitForSeconds(1.0f);
            mAiBuild.SetActionEvent(AIBuild.ActionEvent.None);
            mAiBuild.ChangeState("Waiting");
        }
    }

    public IEnumerator CounterState(float dmg)
    {
        
        if (mTarget.mBuffNerfController.GetBuff(typeof(Counter)))
        {
            Counter counter = mTarget.mBuffNerfController.GetBuff(typeof(Counter)) as Counter;
            if(counter.mChanceRate >= UnityEngine.Random.Range(0.0f, 1.0f))
            {
                yield return new WaitForSeconds(0.25f);
                AudioManager.PlaySfx((mTarget.mAttackClips.Count() > 0) ? mTarget.mAttackClips.ElementAt(Random.Range(0, mTarget.mAttackClips.Count())).Clip : null, 0.6f);
                mTarget.mTarget = this;
                mTarget.mAnimator.Play((mTarget.MyAttackAnim.Count > 0) ? mTarget.MyAttackAnim[Random.Range(0, mTarget.MyAttackAnim.Count)] : "Attack");
                mTarget.mTarget.TakeDamage(dmg, DamageType.Physical);
                if (mStatus.mHealth <= 0.0f)
                {
                    mAiBuild.actionEvent = AIBuild.ActionEvent.Busy;
                    mGroundCheck.SetActive(false);
                }
            }
        }
    }
    
    public bool DodgeState()
    {
        if (mBuffNerfController.GetBuff(typeof(Dodge)))
        {
            Dodge dodge = mBuffNerfController.GetBuff(typeof(Dodge)) as Dodge;
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
            value = (value - (mStatus.mArmor + mBonusStatus.mArmor) <= 0.0f) ? ((mConditions.isDefend) ? 0.0f : 1.0f) : value - (mStatus.mArmor + mBonusStatus.mArmor);
        }
        else
        {
            value = (value - (mStatus.mMagic_Resistance + mBonusStatus.mMagic_Resistance) <= 0.0f) ? 1.0f : value - (mStatus.mMagic_Resistance + mBonusStatus.mMagic_Resistance);
            isDodge = false;
        }

        value = Mathf.Round(value);

        if (isDodge)
        {
            mAiBuild.actionEvent = AIBuild.ActionEvent.DodgeWalk;
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mDodgeSFX);
        }
        DisplayDamage(isDodge, value);
        if (!isDodge)
        {
            GameObject go = Instantiate(ResourceManager.GetResource<GameObject>
                ("Prefabs/Effects/Hit"), transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.2f), Random.Range(-1.0f, 1.0f)), Quaternion.identity, transform);
            Destroy(go, 1.1f);
            mStatus.mHealth -= value;
            mHealthBar.mCurrentHealth = mStatus.mHealth + mBonusStatus.mHealth;
            mHealthBar.StartCoroutine(mHealthBar.PlayBleed());
            if (mStatus.mHealth <= 0.0f)
            {
                if (mDeathClips.Count() > 0)
                    AudioManager.PlaySfx(mDeathClips.ElementAt(Random.Range(0, mDeathClips.Count())).Clip, 0.6f);

                mSelected.SetActive(false);
                mConditions.isDied = true;
                mLevelText.gameObject.SetActive(false);
                mStatus.mHealth = mHealthBar.mCurrentHealth = 0.0f;
                if (mFlag == Flag.Enemy)
                {
                    GameManager.s_TotalExp += mStatus.mEXP;
                    GameManager.s_TotalGold += mStatus.mGold;
                    GameManager.s_TotalSoul += GameManager.Instance.mAmountofSoul;
                }
                mirror?.SetBool("Death", true);
                mAnimator.SetBool("Death", true);
                mHealthBar.ActiveDeathAnimation(true);
                GetComponent<BoxCollider>().enabled = mField.IsExist = false;
                UIManager.Instance.mStorage.mOrderbar.GetComponent<OrderBar>().DequeueOrder(this);
                mBuffNerfController.Stop();
                mField.Picked(false);
            }
            mirror?.SetTrigger("Hit");
            mAnimator.SetTrigger("Hit");
        }

        return !isDodge;
    }

    private void DisplayDamage(bool isDodge, float value)
    {
        GameObject damage = Instantiate(mCanvas.transform.Find("DamageValue").gameObject
            , mCanvas.transform.position - new Vector3(0.0f, 1.0f, 0.0f), Quaternion.identity, mCanvas.transform);
        damage.SetActive(true);
        damage.GetComponent<TextMeshProUGUI>().text = (isDodge) ? "Miss!" : ((value == 0.0f) ? "Blocked!" : value.ToString());
        Destroy(damage, 1.1f);
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
        if (mStatus.mHealth + val >= mStatus.mMaxHealth + mBonusStatus.mHealth)
            mStatus.mHealth = mStatus.mMaxHealth + mBonusStatus.mHealth;
        else
            mStatus.mHealth += val;
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
        mConditions.isBattleComplete = true;
        if(mTarget != null)
            mTarget.mSelected.SetActive(false);
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

    virtual public void DisableUnit(Vector3 pos)
    {
        transform.position = pos;
        mHealthBar.Active(false);

        mStatus.mMaxHealth -= mBonusStatus.mHealth;
        mStatus.mHealth -= mBonusStatus.mHealth;

        mStatus.mMaxMana -= mBonusStatus.mMana;
        mStatus.mMana -= mBonusStatus.mMana;

        mBuffNerfController.Stop();
        mAiBuild.SetActionEvent(AIBuild.ActionEvent.None);
        gameObject.SetActive(false);
    }

    virtual public void EnableUnit(int index)
    {
        if(mFlag == Flag.Player)
        {
            transform.position = BattleManager.Instance.playerCenter;
            mField = BattleManager.playerFieldParent.GetChild(index).GetComponent<Field>();
        }
        else
        {
            transform.position = BattleManager.Instance.enemyCenter;
            mField = BattleManager.enemyFieldParent.GetChild(index).GetComponent<Field>();
        }
        mField.mUnit = this;
        mStatus.mHealth += mBonusStatus.mHealth;
        mStatus.mMaxHealth += mBonusStatus.mHealth;

        mStatus.mMana += mBonusStatus.mMana;
        mStatus.mMaxMana += mBonusStatus.mMana;

        mAiBuild.SetActionEvent(AIBuild.ActionEvent.IntroWalk);

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

    virtual public void BeginTurn()
    {
        mConditions.isDefend = false;
        mConditions.isBattleComplete = false;
        mTarget = null;
        mOrder = Order.Standby;
        mBuffNerfController.Tick();
        mField.Picked(true);
    }
}
