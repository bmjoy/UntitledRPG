using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class BattleManager : MonoBehaviour
{
    enum GameStatus
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
    public bool isBattle = false;
    private bool isWin = false;
    private bool onReward = false;
    private GameStatus status = GameStatus.None;

    [SerializeField]
    private float mWaitTime;

    public event Action onEnqueuingOrderEvent;
    public event Action<Unit> onDequeuingOrderEvent;
    public event Action onMovingOrderEvent;
    public event Action onFinishOrderEvent;

    public void Initialize()
    {
        isBattle = true;
        for (int i = 0; i < GameManager.Instance.EnemyProwlers.Length; ++i)
        {
            if(GameManager.Instance.EnemyProwlers[i] != null)
            {
                if (GameManager.Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().id == GameManager.Instance.mEnemyProwler.id)
                    continue;
                GameManager.Instance.EnemyProwlers[i].SetActive(false);
                GameManager.Instance.EnemyProwlers[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
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
        if (!isBattle)
            return;
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
                            mCurrentUnit = null;
                            return;
                        }
                        else
                        {
                            mCurrentUnit.mConditions.isDefend = false;
                            mCurrentUnit.mConditions.isPicked = true;
                            onMovingOrderEvent?.Invoke();
                            mCurrentUnit.mOrder = Order.Standby;
                            if (mCurrentUnit.mFlag == Flag.Player)
                            {
                                UIManager.Instance.DisplayBattleInterface(true);
                                if (mCurrentUnit.mSkillDataBase != null)
                                    if (mCurrentUnit.mSkillDataBase.mSkill != null)
                                        UIManager.Instance.ChangeText(mCurrentUnit.mSkillDataBase.mSkill.mName + ": \n" + mCurrentUnit.mSkillDataBase.mSkill.mDescription);
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
                    if (status == GameStatus.Reward)
                        UIManager.Instance.mFadeScreen.SetActive(true);
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
                    isBattle = false;
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
        int exp = GameManager.Instance.s_TotalExp;
        int gold = GameManager.Instance.s_TotalGold;
        UIManager.Instance.DisplayAskingSkill(true);
        UIManager.Instance.FadeInWord();
        UIManager.Instance.ChangeText_Target("Victory! \n EXP: " + exp + "\n Gold: " + gold);
        int shareExp = GameManager.Instance.s_TotalExp / GameManager.Instance.mPlayer.mHeroes.Count;
        foreach (var unit in GameManager.Instance.mPlayer.mHeroes)
        {
            unit.GetComponent<Unit>().mStatus.mEXP += shareExp;
        }
        GameManager.Instance.mPlayer.mGold += GameManager.Instance.s_TotalGold;

        yield return new WaitForSeconds(mWaitTime);
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.FadeOutWord();
        yield return new WaitForSeconds(1.0f);
        UIManager.Instance.mSkillUseCheck.SetActive(false);
        UIManager.Instance.mFadeScreen.SetActive(false);
        GameManager.Instance.s_TotalExp = GameManager.Instance.s_TotalGold = 0;
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
        NavMeshHit meshHit = new NavMeshHit();
        Vector3 point = mOffset + 0.5f * (mTargetOffset - mOffset);

        if(!CheckFieldAvailable(point + new Vector3(0.0f, 0.0f, -13.0f), ref meshHit) &&
            !CheckFieldAvailable(point + new Vector3(0.0f, 0.0f, 13.0f), ref meshHit))
        {}
        else if (!CheckFieldAvailable(point + new Vector3(0.0f, 0.0f, -13.0f), ref meshHit))
            point += new Vector3(0.0f, 0.0f, 10.0f);
        else if (!CheckFieldAvailable(point + new Vector3(0.0f, 0.0f, 13.0f), ref meshHit))
            point += new Vector3(0.0f, 0.0f, -10.0f);
        else
        { }

        GameManager.Instance.mCurrentField.transform.localPosition = point;
        AdjustBattleField();
        CameraSwitcher.UpdateCamera(GameManager.Instance.mCurrentField.transform);
        GameManager.Instance.mCurrentField.SetActive(true);
    }

    public void AdjustBattleField()
    {
        NavMeshHit meshHit = new NavMeshHit();
        Transform playerFieldParent = GameManager.Instance.mCurrentField.transform.Find("PlayerFields");
        Transform enemyFieldParent = GameManager.Instance.mCurrentField.transform.Find("EnemyFields");

        playerCenter = playerFieldParent.position;
        enemyCenter = enemyFieldParent.position;

        if (!CheckFieldAvailable(playerFieldParent.position, ref meshHit))
        {
            NavMesh.SamplePosition(playerFieldParent.position, out meshHit, 50.0f, 3);
            playerFieldParent.position = meshHit.position;
        }

        if (!CheckFieldAvailable(enemyFieldParent.position, ref meshHit))
        {
            NavMesh.SamplePosition(enemyFieldParent.position, out meshHit, 50.0f, 3);
            enemyFieldParent.position = meshHit.position;
        }

        foreach (Transform playerField in playerFieldParent)
        {
            if (!CheckFieldAvailable(playerField.position, ref meshHit))
            {
                NavMesh.SamplePosition(playerField.position, out meshHit, 50.0f, 3);
                playerField.position = meshHit.position;
            }
        }
        foreach (Transform enemyField in enemyFieldParent)
        {
            if (!CheckFieldAvailable(enemyField.position, ref meshHit))
            {
                NavMesh.SamplePosition(enemyField.position, out meshHit, 50.0f, 3);
                enemyField.position = meshHit.position;
            }
        }
    }

    private bool CheckFieldAvailable(Vector3 pos, ref NavMeshHit navMeshHit)
    {
        return NavMesh.SamplePosition(pos, out navMeshHit, 1.0f, NavMesh.AllAreas);
    }

    public void Attack()
    {   
        if (status == GameStatus.WaitForOrder)
        {
            StartCoroutine(mCurrentUnit.AttackAction(DamageType.Physical, () => { status = GameStatus.Busy; }));
            UIManager.Instance.DisplayBattleInterface(false);
        }
    }

    public void Defend()
    {
        if(status == GameStatus.WaitForOrder)
        {
            Debug.Log("Defend");
            StartCoroutine(mCurrentUnit.DefendAction(() => { status = GameStatus.Busy; }));
            UIManager.Instance.DisplayBattleInterface(false);
        }
    }

    public void Magic()
    {
        if(status == GameStatus.WaitForOrder)
        {
            Debug.Log("Magic");
            StartCoroutine(mCurrentUnit.MagicAction(() => { status = GameStatus.Busy; }));
            UIManager.Instance.DisplayBattleInterface(false);
        }
    }

    public void Cancel()
    {
        status = GameStatus.WaitForOrder;
        UIManager.Instance.DisplayBattleInterface(true);
        mCurrentUnit.StopAllCoroutines();
        mCurrentUnit.mTarget = null;
        Debug.Log("Cancel");
    }
}
