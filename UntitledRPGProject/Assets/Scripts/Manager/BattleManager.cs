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
        Debug.Log("Hi BattleManager");
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public List<GameObject> mUnits = new List<GameObject>();
    public Queue<Unit> mOrders = new Queue<Unit>();
    public Vector3 playerCenter = Vector3.zero;
    public Vector3 enemyCenter = Vector3.zero;

    private Unit mCurrentUnit = null;
    private bool isWin = false;
    private bool onReward = false;
    public GameStatus status = GameStatus.None;

    [SerializeField]
    private float mWaitTime;

    public event Action onEnqueuingOrderEvent;
    public event Action<Unit> onDequeuingOrderEvent;
    public event Action onMovingOrderEvent;
    public event Action onFinishOrderEvent;

    public void Initialize()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitUntil(()=> GameManager.Instance.mPlayer.onBattle == true 
        && GameManager.Instance.mEnemyProwler.onBattle == true);
        mUnits.Clear();
        mOrders.Clear();
        mUnits.AddRange(GameManager.Instance.mPlayer.mHeroes);
        mUnits.AddRange(GameManager.Instance.mEnemyProwler.mEnemySpawnGroup);
        mUnits.Sort((a, b) => (b.GetComponent<Unit>().mStatus.mAgility.CompareTo(
            a.GetComponent<Unit>().mStatus.mAgility)));
        for (int i = 0; i < mUnits.Count; i++)
        {
            mUnits[i].GetComponent<Unit>().mOrder = Order.Standby;
            mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
        }
        onEnqueuingOrderEvent?.Invoke();
        status = GameStatus.Start;
    }

    private void Update()
    {
        switch (status)
        {
            case GameStatus.None: break;
            case GameStatus.Start:
                {
                    status = (mUnits.TrueForAll(t => t.GetComponent<Unit>().mAiBuild.actionEvent == ActionEvent.None)) ? GameStatus.Queue : GameStatus.Start;
                }
                break;
            case GameStatus.Queue:
                {
                    if (mCurrentUnit)
                        onDequeuingOrderEvent?.Invoke(mCurrentUnit);
                    mCurrentUnit?.mField.GetComponent<Field>().Picked(false);
                    mCurrentUnit = null;
                    if (mOrders.Count == 0 && mCurrentUnit == null)
                    {
                        for (int i = 0; i < mUnits.Count; i++)
                        {
                            if (!mUnits[i].GetComponent<Unit>().mConditions.isDied)
                                mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
                        }
                        onEnqueuingOrderEvent?.Invoke();
                    }

                    if (mCurrentUnit == null && mOrders.Count != 0)
                    {
                        UIManager.Instance.DisplayBattleInterface(false);
                        mCurrentUnit = mOrders.Dequeue();
                        if (mCurrentUnit.mConditions.isDied)
                        {
                            mCurrentUnit.mField.GetComponent<Field>().Picked(false);
                            mCurrentUnit = null;
                            return;
                        }
                        else
                        {
                            mCurrentUnit.mConditions.isDefend = false;
                            mCurrentUnit.mConditions.isPicked = true;
                            mCurrentUnit.mField.GetComponent<Field>().Picked(true);
                            onMovingOrderEvent?.Invoke();
                            mCurrentUnit.mOrder = Order.Standby;
                            if (mCurrentUnit.mFlag == Flag.Player)
                            {
                                UIManager.Instance.DisplayBattleInterface(true);
                                if (mCurrentUnit.mSkillDataBase != null)
                                    if (mCurrentUnit.mSkillDataBase.Skill != null)
                                        UIManager.Instance.ChangeText(mCurrentUnit.mSkillDataBase.Name + ": \n" + mCurrentUnit.mSkillDataBase.Description);
                            }
                            status = GameStatus.WaitForOrder;
                        }
                    }
                }
                break;
            case GameStatus.WaitForOrder:
                break;
            case GameStatus.Busy:
                status = (mCurrentUnit.mOrder == Order.TurnEnd) ? GameStatus.Result : GameStatus.Busy;
                break;
            case GameStatus.Result:
                {
                    status = (BattleResult() == true) ? status = GameStatus.Reward : GameStatus.Queue;
                    break;
                }
            case GameStatus.Reward:
                {
                    if(onReward == false)
                    {
                        onFinishOrderEvent?.Invoke();
                        StartCoroutine(RewardTime());
                        onReward = true;
                    }
                }
                break;
            case GameStatus.Finish:
                {
                    CameraSwitcher.SwitchCamera();
                    GameManager.Instance.mGameState = (isWin) ? GameState.Victory : GameState.GameOver;
                    onReward = false;
                    mCurrentUnit = null;
                    status = GameStatus.None;
                }
                break;
        }
    }

    private IEnumerator RewardTime()
    {
        UIManager.Instance.FadeInScreen();
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.FadeInWord();
        UIManager.Instance.ChangeText("Victory! \n\n EXP: " + GameManager.s_TotalExp + "\n\n Gold: " + GameManager.s_TotalGold);
        int shareExp = GameManager.s_TotalExp / GameManager.Instance.mPlayer.mHeroes.Count;
        foreach (var unit in GameManager.Instance.mPlayer.mHeroes)
        {
            unit.GetComponent<Unit>().mStatus.mEXP += shareExp;
        }
        GameManager.Instance.mPlayer.mGold += GameManager.s_TotalGold;

        yield return new WaitForSeconds(mWaitTime);
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.FadeOutWord();
        GameManager.s_TotalExp = GameManager.s_TotalGold = 0;
        status = GameStatus.Finish;
        yield return null;
    }

    private bool BattleResult()
    {
        if (GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
        {
            isWin = true;
            return true;
        }
        else if (GameManager.Instance.mPlayer.mHeroes.TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
        {
            isWin = false;
            return true;
        }
        else
            return false;
    }

    public void SetBattleField()
    {
        Vector3 mOffset = GameManager.Instance.mPlayer.transform.localPosition;
        Vector3 mTargetOffset = GameManager.Instance.mEnemyProwler.transform.localPosition;
        Vector3 point = mOffset + 0.5f * (mTargetOffset - mOffset);

        GameManager.Instance.mCurrentField.transform.localPosition = point;
        AdjustBattleField();
        CameraSwitcher.UpdateCamera(GameManager.Instance.mCurrentField.transform);
        GameManager.Instance.mCurrentField.SetActive(true);
    }

    public void AdjustBattleField()
    {
        Transform playerFieldParent = GameManager.Instance.mCurrentField.transform.Find("PlayerFields");
        Transform enemyFieldParent = GameManager.Instance.mCurrentField.transform.Find("EnemyFields");

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
    }

    public void Attack()
    {   
        if (status == GameStatus.WaitForOrder)
        {
            StartCoroutine(mCurrentUnit.AttackAction(DamageType.Physical, () => 
            { 
                status = GameStatus.Busy;
                mCurrentUnit.mField.GetComponent<Field>().Stop();
            }));
            UIManager.Instance.DisplayBattleInterface(false);
        }
    }

    public void Defend()
    {
        if(status == GameStatus.WaitForOrder)
        {
            Debug.Log("Defend");
            StartCoroutine(mCurrentUnit.DefendAction(() => 
            { 
                status = GameStatus.Busy;
                mCurrentUnit.mField.GetComponent<Field>().Stop();
            }));
            UIManager.Instance.DisplayBattleInterface(false);
        }
    }

    public void Magic()
    {
        if(status == GameStatus.WaitForOrder)
        {
            if(mCurrentUnit.mSkillDataBase.Skill == null)
            {
                Cancel();
                return;
            }
            Debug.Log("Magic");
            StartCoroutine(mCurrentUnit.MagicAction(() => 
            { 
                status = GameStatus.Busy;
                mCurrentUnit.mField.GetComponent<Field>().Stop();
            }));
            UIManager.Instance.DisplayBattleInterface(false);
        }
    }

    public void Cancel()
    {
        status = GameStatus.WaitForOrder;
        UIManager.Instance.DisplayBattleInterface(true);
        mCurrentUnit.StopAllCoroutines();
        mCurrentUnit.mTarget = null;
        mCurrentUnit.mField.GetComponent<Field>().Picked(true);
        Debug.Log("Cancel");
    }
}
