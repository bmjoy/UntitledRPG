using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class BattleManager : MonoBehaviour
{
    public enum GameStatus
    {
        None,
        Start,
        Queue,
        WaitForOrder,
        Busy,
        Result,
        Reward,
        Finish
    }

    private static BattleManager mInstance;
    public static BattleManager Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    public GameObject mCurrentField;
    public List<GameObject> mUnits = new List<GameObject>();
    public Queue<Unit> mOrders = new Queue<Unit>();
    public Vector3 playerCenter = Vector3.zero;
    public Vector3 enemyCenter = Vector3.zero;

    private List<Vector3> mOriginalFieldPos = new List<Vector3>(8)
    {
        new Vector3(0.0f,0.0f, -6.0f),
        new Vector3(0.0f,0.0f,-13.0f),
        new Vector3(-5.0f,0.0f,-10.0f),
        new Vector3(5.0f,0.0f,-10.0f),
        new Vector3(0.0f,0.0f, 6.0f),
        new Vector3(0.0f,0.0f,13.0f),
        new Vector3(-5.0f,0.0f,10.0f),
        new Vector3(5.0f,1.0f,10.0f),
    };

    public Unit mCurrentUnit = null;
    public float mPercentageHP = 30.0f;
    private bool isWin = false;
    private bool onReward = false;
    public GameStatus status = GameStatus.None;

    [SerializeField]
    private float mWaitTime;

    public event Action onEnqueuingOrderEvent;
    public event Action<Unit> onDequeuingOrderEvent;
    public event Action<Unit> onEnqueuingSingleOrderEvent;
    public event Action onMovingOrderEvent;
    public event Action onFinishOrderEvent;

    private void Start()
    {
        mCurrentField = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Field"));
        mCurrentField.transform.parent = transform;
        mCurrentField.SetActive(false);
        GameManager.Instance.onPlayerBattleStart += Initialize;
    }

    public void Initialize()
    {
        UIManager.Instance.DisplayInventory(false);
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitUntil(()=> PlayerController.Instance.onBattle == true 
        && GameManager.Instance.mEnemyProwler.onBattle == true);
        mUnits.Clear();
        mOrders.Clear();
        mUnits.AddRange(PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false));
        mUnits.AddRange(GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false));
        mUnits.Sort((a, b) => ((b.GetComponent<Unit>().mStatus.mAgility + b.GetComponent<Unit>().mBonusStatus.mAgility).CompareTo(
            a.GetComponent<Unit>().mStatus.mAgility + a.GetComponent<Unit>().mBonusStatus.mAgility)));
        for (int i = 0; i < mUnits.Count; i++)
        {
            mUnits[i].GetComponent<Unit>().mOrder = Order.Standby;
            mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
        }
        onEnqueuingOrderEvent?.Invoke();
        status = GameStatus.Start;
    }

    float mTime = 0.0f;
    float mPendingTime = 2.0f;

    private void Update()
    {
        switch (status)
        {
            case GameStatus.None: break;
            case GameStatus.Start:
                {
                    status = (mUnits.TrueForAll(t => t.GetComponent<Unit>().mAiBuild.actionEvent == ActionEvent.None) && mTime >= mPendingTime) ? GameStatus.Queue : GameStatus.Start;
                    mTime += Time.deltaTime;
                }
                break;
            case GameStatus.Queue:
                {
                    UIManager.Instance.DisplayBattleInterface(false);
                    mCurrentUnit = mOrders.Dequeue();

                    if (mCurrentUnit.mConditions.isDied)
                    {
                        mCurrentUnit.mAiBuild.stateMachine.ChangeState("Waiting");
                        mCurrentUnit.mField.GetComponent<Field>().Picked(false);
                        onDequeuingOrderEvent?.Invoke(mCurrentUnit);
                        mCurrentUnit = null;
                        return;
                    }
                    else
                    {
                        mCurrentUnit.mAiBuild.stateMachine.ChangeState("Standby");
                        onMovingOrderEvent?.Invoke();
                        UIManager.Instance.DisplayBattleInterface((mCurrentUnit.mFlag == Flag.Player) ? true : false);
                        var data = mCurrentUnit.GetComponent<Skill_DataBase>();
                        if (data != null)
                            UIManager.Instance.ChangeHoverTip((data.Skill) ? "<b><color=red>" + data.Name + "</color></b>: " + data.Description : "Empty","Skill");
                        UIManager.Instance.ChangeHoverTip("This unit can give <b><color=red>" + mCurrentUnit.mStatus.mDamage + "</color>Damage</b>!", "Attack");
                        UIManager.Instance.ChangeHoverTip("This unit has <b>" + mCurrentUnit.mStatus.mArmor + " Armors </b> and " +
                            "<b>" + "Defend <color=green>" + mCurrentUnit.mStatus.mDefend + "%</color></b> can block damages", "Defend");
                        status = (BattleResult() == true) ? GameStatus.Reward : GameStatus.WaitForOrder;
                    }
                }
                break;
            case GameStatus.WaitForOrder:
                break;
            case GameStatus.Busy:
                if(mCurrentUnit.mOrder == Order.TurnEnd)
                {
                    status = GameStatus.Result;
                    if (!mCurrentUnit.mConditions.isDied)
                        mOrders.Enqueue(mCurrentUnit);
                    onDequeuingOrderEvent?.Invoke(mCurrentUnit);
                    if(!mCurrentUnit.mConditions.isDied)
                        onEnqueuingSingleOrderEvent?.Invoke(mCurrentUnit);
                }
                break;
            case GameStatus.Result:
                {
                    status = (BattleResult() == true) ? status = GameStatus.Reward : GameStatus.Queue;
                }
                break;
            case GameStatus.Reward:
                {
                    UIManager.Instance.DisplayBattleInterface(false);

                    if (onReward == false && isWin)
                    {
                        onFinishOrderEvent?.Invoke();
                        StartCoroutine(RewardTime());
                        onReward = true;
                    }
                    if (!isWin)
                    {
                        status = GameStatus.Finish;
                        UIManager.Instance.BattleEnd();
                    }
                    if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) && _AvailableSkip)
                    {
                        StopCoroutine(RewardTime());
                        int shareExp = GameManager.s_TotalExp / PlayerController.Instance.mHeroes.Count;
                        foreach (var unit in PlayerController.Instance.mHeroes)
                            unit.GetComponent<Unit>().mStatus.mEXP += shareExp;
                        UIManager.Instance.mVictoryScreen.Active(true);
                        GetEnemyItem();
                        PlayerController.Instance.mGold += GameManager.s_TotalGold;
                        UIManager.Instance.FadeOutScreen();
                        GameManager.s_TotalSoul = GameManager.s_TotalExp = GameManager.s_TotalGold = 0;
                        UIManager.Instance.mVictoryScreen.StartCoroutine(UIManager.Instance.mVictoryScreen.WaitForEnd());
                        status = GameStatus.Finish;
                    }
                }
                break;
            case GameStatus.Finish:
                {
                    foreach (GameObject unit in mUnits)
                        unit.GetComponent<Unit>().ClearBuffAndNerf();
                    UIManager.Instance.DisplayHealthBar(false);
                    GameManager.Instance.mGameState = (isWin) ? GameState.Victory : GameState.GameOver;
                    onReward = false;
                    mCurrentUnit = null;
                    _AvailableSkip = false;
                    mTime = 0.0f;
                    status = GameStatus.None;
                }
                break;
        }
    }

    List<Enemy> enemyList = new List<Enemy>();
    List<GameObject> enemyItemList = new List<GameObject>();
    private void GetEnemyItem()
    {
        enemyList.Clear();
        enemyItemList.Clear();
        foreach(GameObject unit in mUnits.Where(x => x.GetComponent<Enemy>()).ToList())
        {
            enemyList.Add(unit.GetComponent<Enemy>());
        }
        foreach (Enemy enemy in enemyList)
        {
            if(UnityEngine.Random.Range(0,100) < enemy.mItemDropRate)
            {
                enemyItemList.AddRange(enemy.mSetting.Item);
            }
        }

        UIManager.Instance.mVictoryScreen.UpdateItemList(enemyItemList);

        foreach(var item in enemyItemList)
        {
            GameObject i = Instantiate(item);
            i.transform.SetParent(PlayerController.Instance.transform.Find("Beg"));
            PlayerController.Instance.mInventory.Add(i.GetComponent<Item>());
        }

    }

    bool _AvailableSkip = false;
    [SerializeField]
    float _TransitionTime = 2.1f;

    private IEnumerator RewardTime()
    {
        UIManager.Instance.FadeInScreen();
        UIManager.Instance.StartCoroutine(UIManager.Instance.VictoryTransition());
        yield return new WaitForSeconds(_TransitionTime);
        _AvailableSkip = true;
        int shareExp = GameManager.s_TotalExp / PlayerController.Instance.mHeroes.Count;
        foreach (var unit in PlayerController.Instance.mHeroes)
            unit.GetComponent<Unit>().mStatus.mEXP += shareExp;
        UIManager.Instance.mVictoryScreen.Active(true);
        GetEnemyItem();
        PlayerController.Instance.mGold += GameManager.s_TotalGold;
        yield return new WaitForSeconds(mWaitTime);
        UIManager.Instance.FadeOutScreen();
        GameManager.s_TotalSoul = GameManager.s_TotalExp = GameManager.s_TotalGold = 0;
        UIManager.Instance.mVictoryScreen.StartCoroutine(UIManager.Instance.mVictoryScreen.WaitForEnd());
        status = GameStatus.Finish;

    }

    private bool BattleResult()
    {
        if (Instance.mUnits.Where(t => t.GetComponent<Unit>().mFlag == Flag.Enemy).ToList().TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
        {
            isWin = true;
            return true;
        }
        else if (Instance.mUnits.Where(t => t.GetComponent<Unit>().mFlag == Flag.Player).ToList().TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
        {
            isWin = false;
            return true;
        }
        else
            return false;
    }

    public void SetBattleField()
    {
        Vector3 mOffset = PlayerController.Instance.transform.localPosition;
        Vector3 mTargetOffset = GameManager.Instance.mEnemyProwler.transform.localPosition;
        Vector3 point = mOffset + 0.5f * (mTargetOffset - mOffset);

        Instance.mCurrentField.transform.localPosition = point;
        AdjustBattleField();
    }

    public static Transform playerFieldParent;
    public static Transform enemyFieldParent;

    public void AdjustBattleField()
    {
        playerFieldParent = Instance.mCurrentField.transform.Find("PlayerFields");
        enemyFieldParent = Instance.mCurrentField.transform.Find("EnemyFields");

        playerCenter = playerFieldParent.position;
        enemyCenter = enemyFieldParent.position;

        foreach (Transform playerField in playerFieldParent)
        {
            playerField.GetComponent<Field>().Initialize();
        }
        foreach (Transform enemyField in enemyFieldParent)
        {
            enemyField.GetComponent<Field>().Initialize();
        }

        CameraSwitcher.UpdateCamera(Instance.mCurrentField.transform);
        Instance.mCurrentField.SetActive(true);
    }

    public void ResetField()
    {
        for (int i = 0; i < Instance.mCurrentField.transform.Find("PlayerFields").childCount; ++i)
        {
            Instance.mCurrentField.transform.Find("PlayerFields").GetChild(i).transform.localPosition = mOriginalFieldPos[i];
            Instance.mCurrentField.transform.Find("PlayerFields").GetChild(i).GetComponent<Field>().IsExist = false;
        }

        for (int i = 0; i < Instance.mCurrentField.transform.Find("EnemyFields").childCount; ++i)
        {
            Instance.mCurrentField.transform.Find("EnemyFields").GetChild(i).transform.localPosition = mOriginalFieldPos[i + 4];
            Instance.mCurrentField.transform.Find("EnemyFields").GetChild(i).GetComponent<Field>().IsExist = false;
        }
        Instance.mCurrentField.SetActive(false);
    }

    public void Attack()
    {
        if (status == GameStatus.WaitForOrder)
        {
            UIManager.Instance.DisplayBattleInterface(false);
            StartCoroutine(mCurrentUnit.AttackAction(DamageType.Physical, () => EndAction()));
        }
    }

    public void Defend()
    {
        if(status == GameStatus.WaitForOrder)
        {
            UIManager.Instance.DisplayBattleInterface(false);
            StartCoroutine(mCurrentUnit.DefendAction(() => EndAction()));
        }
    }

    public void Magic()
    {
        if(status == GameStatus.WaitForOrder)
        {
            if(mCurrentUnit.GetComponent<Skill_DataBase>().Skill == null)
            {
                Cancel();
                return;
            }
            UIManager.Instance.DisplayBattleInterface(false);
            StartCoroutine(mCurrentUnit.MagicAction(() => EndAction()));
        }
    }

    private void EndAction()
    {
        status = GameStatus.Busy;
        mCurrentUnit.mField.GetComponent<Field>().Stop();
    }

    public void Cancel()
    {
        mCurrentUnit.mConditions.isCancel = false;
        status = GameStatus.WaitForOrder;
        UIManager.Instance.DisplayBattleInterface(true);
        mCurrentUnit.StopAllCoroutines();
        mCurrentUnit.mTarget = null;
        mCurrentUnit.mField.GetComponent<Field>().Picked(true);
    }
}
