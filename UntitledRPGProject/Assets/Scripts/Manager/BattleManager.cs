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
    [HideInInspector]
    public GameObject mCurrentField;
    [HideInInspector]
    public List<GameObject> mUnits = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> mEnemies = new List<GameObject>();
    [HideInInspector]
    public Queue<Unit> mOrders = new Queue<Unit>();
    public Vector3 playerCenter = Vector3.zero;
    public Vector3 enemyCenter = Vector3.zero;
    public float mRunningSpeed = 9.0f;
    [HideInInspector]
    public bool mSpellChanning = false;

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

    [SerializeField]
    private float _TransitionTime = 2.1f;
    private bool isWin = false;
    public bool onReward = false;
    private bool _AvailableSkip = false;
    public GameStatus status = GameStatus.None;

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
        if(UIManager.Instance.mInventoryUI.transform.gameObject.activeSelf)
            UIManager.Instance.DisplayInventory(false);

        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitUntil(()=> PlayerController.Instance.onBattle == true 
        && GameManager.Instance.mEnemyProwler.onBattle == true);
       
        mUnits.Clear();
        mOrders.Clear();
        mEnemies.Clear();
        mUnits.AddRange(PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false));
        mEnemies = GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList();
        if (!mEnemies.Exists(s => s.GetComponent<Boss>()))
        {
            AudioManager.Instance.mAudioStorage.ChangeMusic("Battle");
            AudioManager.Instance.musicSource.loop = true;
        }
        mUnits.AddRange(GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false));
        mUnits.Sort((a, b) => ((b.GetComponent<Unit>().mStatus.mAgility + b.GetComponent<Unit>().mBonusStatus.mAgility).CompareTo(
            a.GetComponent<Unit>().mStatus.mAgility + a.GetComponent<Unit>().mBonusStatus.mAgility)));
        for (int i = 0; i < mUnits.Count; i++)
        {
            mUnits[i].GetComponent<Unit>().mOrder = Order.Standby;
            mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
        }
        UIManager.Instance.DisplayHealthBar(true);
        UIManager.Instance.ChangeOrderBarText("");
        onEnqueuingOrderEvent?.Invoke();
        status = GameStatus.Start;
        yield return new WaitForSeconds(2.0f);
        //CameraSwitcher.CollideCheck();
        //CameraSwitcher.isInitialized = true;
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
                    status = (mUnits.TrueForAll(t => t.GetComponent<Unit>().mAiBuild.actionEvent == AIBuild.ActionEvent.None) && mTime >= mPendingTime) ? GameStatus.Queue : GameStatus.Start;
                    mTime += Time.deltaTime;
                }
                break;
            case GameStatus.Queue:
                {
                    UIManager.Instance.DisplayBattleInterface(false);
                    mCurrentUnit = mOrders.Dequeue();

                    if (mCurrentUnit.mConditions.isDied)
                    {
                        mCurrentUnit.mAiBuild.ChangeState("Waiting");
                        mCurrentUnit.mField.GetComponent<Field>().Picked(false);
                        onDequeuingOrderEvent?.Invoke(mCurrentUnit);
                        mCurrentUnit = null;
                        return;
                    }
                    else
                    {
                        mCurrentUnit.BeginTurn();
                        if (mCurrentUnit.mAiBuild.type == AIBuild.AIType.Auto)
                            mCurrentUnit.mAiBuild.ChangeState("Standby");
                        onMovingOrderEvent?.Invoke();
                        mCurrentUnit.mField.GetComponent<Field>().Picked(true);
                        if (mCurrentUnit.mAiBuild.type == AIBuild.AIType.Manual)
                        {
                            UIManager.Instance.DisplaySupportKey(true, true);
                            UIManager.Instance.ChangeSupportText(new string[3]
                            {
                                "Proceed",
                                "Cancel",
                                string.Empty
                            });
                            UIManager.Instance.DisplayBattleInterface((mCurrentUnit.mFlag == Flag.Player) ? true : false);
                            var data = mCurrentUnit.GetComponent<Skill_DataBase>();
                            if (data != null)
                                UIManager.Instance.ChangeHoverTip((data.Skill) ? "<b><color=red>" + data.ToString() + "</color></b>: " + data.Description : "Empty", "Skill");
                            UIManager.Instance.ChangeHoverTip("This unit can give <b><color=red>" + mCurrentUnit.mStatus.mDamage + "</color>(<color=green>+" + mCurrentUnit.mBonusStatus.mDamage + "</color>)Damage</b>!", "Attack");
                            UIManager.Instance.ChangeHoverTip("This unit has <b>" + mCurrentUnit.mStatus.mArmor + " Armors </b>(<color=green>+" + mCurrentUnit.mBonusStatus.mArmor + "</color>) and " +
                                "<b>" + "Defend <color=green>" + mCurrentUnit.mStatus.mDefend + "%</color></b> can block damages", "Defend");
                        }
                        status = (BattleResult() == true) ? GameStatus.Reward : GameStatus.WaitForOrder;
                        UIManager.Instance.ChangeOrderBarText("Waiting for Order...");
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
                        UIManager.Instance.DisplaySupportKey(false);
                        UIManager.Instance.DisplayHealthBar(false);
                        AudioManager.Instance.mAudioStorage.ChangeMusic("Victory");
                        AudioManager.Instance.musicSource.loop = false;
                        onFinishOrderEvent?.Invoke();
                        StartCoroutine(RewardTime());
                        onReward = true;
                    }
                    if(Input.GetKeyDown(KeyCode.E) && _AvailableSkip)
                    {
                        StopCoroutine(RewardTime());

                        UIManager.Instance.FadeOutScreen();
                        GameManager.s_TotalSoul = GameManager.s_TotalExp = GameManager.s_TotalGold = 0;
                        UIManager.Instance.mVictoryScreen.StartCoroutine(UIManager.Instance.mVictoryScreen.WaitForEnd());
                        GameManager.Instance.mGameState = GameState.Victory;
                        status = GameStatus.Finish;
                        _AvailableSkip = false;
                    }
                }
                break;
            case GameStatus.Finish:
                {
                    foreach (GameObject unit in mUnits)
                        unit?.GetComponent<BuffAndNerfEntity>().Stop();
                    UIManager.Instance.DisplayHealthBar(false);
                    CameraSwitcher.StopShakeCamera();
                    mCurrentUnit = null;
                    _AvailableSkip = onReward = false;
                    mTime = 0.0f;
                    status = GameStatus.None;
                }
                break;
        }
        if (mCurrentUnit != null && mCurrentUnit.mConditions.isDied)
        {
            mCurrentUnit.mAiBuild.stateMachine.ChangeState("Waiting");
            mCurrentUnit.mField.GetComponent<Field>().Picked(false);
            UIManager.Instance.DisplayBattleInterface(false);
            onDequeuingOrderEvent?.Invoke(mCurrentUnit);
            mCurrentUnit = null;
            status = GameStatus.Queue;
        }
    }

    List<Enemy> enemyList = new List<Enemy>();
    List<GameObject> enemyItemList = new List<GameObject>();
    private void GetEnemyItem()
    {
        enemyList.Clear();
        enemyItemList.Clear();
        for (int i = 0; i < mEnemies.Count; ++i)
        {
            var unit = mEnemies[i];
            enemyList.Add(unit.GetComponent<Enemy>());
        }

        for (int y = 0; y < enemyList.Count; ++y)
        {
            Enemy enemy = enemyList[y];
            for (int x = 0; x < enemy.mSetting.Item.Count; ++x)
            {
                ItemDrop obj = enemy.mSetting.Item[x];
                if (obj == null) continue;
                if (UnityEngine.Random.Range(0, 100) <= obj.mRate)
                {
                    if(obj.mItem != null)
                        enemyItemList.Add(obj.mItem);
                }
            }
        }

        UIManager.Instance.mVictoryScreen.UpdateItemList(enemyItemList);
        for (int i = 0; i < enemyItemList.Count; ++i)
        {
            GameObject item = Instantiate(enemyItemList[i], PlayerController.Instance.transform.Find("Bag"));
            item.GetComponent<Item>().isSold = true;
            PlayerController.Instance.mInventory.Add(item.GetComponent<Item>());
        }
    }


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
        PlayerController.Instance.mSoul += GameManager.s_TotalSoul;
        GameManager.s_TotalSoul = GameManager.s_TotalExp = GameManager.s_TotalGold = 0;
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
        else return false;
    }

    public void SetBattleField()
    {
        Instance.mCurrentField.transform.localPosition = GameManager.Instance.mEnemyProwler.mySpawner.transform.position;
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

        for (int i = 0; i < playerFieldParent.childCount; i++)
        {
            var playerField = playerFieldParent.GetChild(i);
            playerField.GetComponent<Field>().Initialize();
        }        

        for (int i = 0; i < enemyFieldParent.childCount; i++)
        {
            var enemyField = enemyFieldParent.GetChild(i);
            enemyField.GetComponent<Field>().Initialize();
        }

        CameraSwitcher.UpdateCamera(Instance.mCurrentField.transform);
        Instance.mCurrentField.SetActive(true);
    }

    public void ResetField()
    {
        for (int i = 0; i < playerFieldParent.childCount; ++i)
        {
            playerFieldParent.GetChild(i).transform.localPosition = mOriginalFieldPos[i];
            playerFieldParent.GetChild(i).GetComponent<Field>().IsExist = false;
        }

        for (int i = 0; i < enemyFieldParent.childCount; ++i)
        {
            enemyFieldParent.GetChild(i).transform.localPosition = mOriginalFieldPos[i + 4];
            enemyFieldParent.GetChild(i).GetComponent<Field>().IsExist = false;
        }
        Instance.mCurrentField.SetActive(false);
    }

    public void Attack()
    {
        if (GameManager.Instance.mGameState == GameState.GamePause) return;
        if (status == GameStatus.WaitForOrder)
        {
            UIManager.Instance.DisplayBattleInterface(false);
            StartCoroutine(mCurrentUnit.AttackAction(DamageType.Physical, () =>
            {
                UIManager.Instance.DisplaySupportKey(false);
                EndAction();
            }
            ));
        }
    }

    public void Defend()
    {
        if (GameManager.Instance.mGameState == GameState.GamePause) return;
        if (status == GameStatus.WaitForOrder)
        {
            UIManager.Instance.DisplayBattleInterface(false);
            StartCoroutine(mCurrentUnit.DefendAction(() =>
            {
                UIManager.Instance.DisplaySupportKey(false);
                EndAction();
            }
            ));
        }
    }

    public void Magic()
    {
        if (GameManager.Instance.mGameState == GameState.GamePause) return;
        if (status == GameStatus.WaitForOrder)
        {
            if(mCurrentUnit.GetComponent<Skill_DataBase>().Skill == null)
            {
                Cancel();
                return;
            }
            UIManager.Instance.DisplayBattleInterface(false);
            StartCoroutine(mCurrentUnit.MagicAction(() =>
            {
                EndAction();
            }));
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
        mCurrentUnit.mTarget?.mField.TargetedHostile(false);
        mCurrentUnit.mTarget?.mField.TargetedFriendly(false);
        mCurrentUnit.mTarget?.mSelected.SetActive(false);
        mCurrentUnit.mTarget = null;
        mCurrentUnit.mField.GetComponent<Field>().Picked(true);
        UIManager.Instance.DisplaySupportKey(true,true);
    }
}
