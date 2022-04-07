using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    enum GameStatus
    {
        None,
        Queue,
        WaitForOrder,
        Busy,
        Result,
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
    
    public List<GameObject> mUnits = new List<GameObject>();
    public Queue<Unit> mOrders = new Queue<Unit>();
    private Unit mCurrentUnit = null;
    public bool isBattle = false;
    private bool isWin = false;
    private GameStatus status = GameStatus.None;

    public void Initialize()
    {
        isBattle = true;
        for (int i = 0; i < GameManager.Instance.EnemyProwlers.Length; i++)
        {
            if(GameManager.Instance.EnemyProwlers[i] != null)
            {
                if (GameManager.Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().id == GameManager.Instance.mEnemyProwler.id)
                    continue;

                GameManager.Instance.EnemyProwlers[i].SetActive(false);
            }
        }
        mUnits.Clear();
        mUnits.AddRange(GameManager.Instance.mPlayer.mHeroes);
        mUnits.AddRange(GameManager.Instance.mEnemyProwler.EnemySpawnGroup);
        mUnits.Sort((a, b) => (b.GetComponent<Unit>().mStatus.mAgility.CompareTo(
            a.GetComponent<Unit>().mStatus.mAgility)));
        for (int i = 0; i < mUnits.Count; i++)
        {
            mUnits[i].GetComponent<Unit>().mOrder = Order.Standby;
            mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
        }
        status = GameStatus.Queue;
    }

    private void Update()
    {
        if(isBattle)
        {
            switch (status)
            {
                case GameStatus.None:
                    break;
                case GameStatus.Queue:
                    {
                        mCurrentUnit = null;
                        if (mOrders.Count == 0 && mCurrentUnit == null)
                        {
                            for (int i = 0; i < mUnits.Count; i++)
                            {
                                if (!mUnits[i].GetComponent<Unit>().mConditions.isDied)
                                    mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
                            }
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
                                mCurrentUnit.mOrder = Order.Standby;
                                if (mCurrentUnit.mFlag == Flag.Player)
                                {
                                    UIManager.Instance.DisplayBattleInterface(true);
                                    if(mCurrentUnit.mSkillDataBase != null)
                                        if (mCurrentUnit.mSkillDataBase.mSkill != null)
                                            UIManager.Instance.ChangeText(mCurrentUnit.mSkillDataBase.mSkill.mName + ": \n" + mCurrentUnit.mSkillDataBase.mSkill.mDescription);

                                }
                                status = GameStatus.WaitForOrder;
                                Debug.Log(mCurrentUnit.name);
                            }
                        }
                    }
                    break;
                case GameStatus.WaitForOrder:
                    break;
                case GameStatus.Busy:
                    {
                        status = (mCurrentUnit.mOrder == Order.TurnEnd) ? GameStatus.Result : GameStatus.Busy;
                    }
                    break;
                case GameStatus.Result:
                    {
                        status = (BattleResult() == true) ? status = GameStatus.Finish : GameStatus.Queue;
                    }
                    break;
                case GameStatus.Finish:
                    {
                        CameraSwitcher.SwitchCamera();
                        GameManager.Instance.mGameState = (isWin) ? GameState.Victory : GameState.GameOver;
                        isBattle = false;
                        mCurrentUnit = null;
                        status = GameStatus.None;
                    }
                    break;
                default:
                    break;
            }
        }

    }

    private bool BattleResult()
    {
        if (GameManager.Instance.mEnemyProwler.EnemySpawnGroup.TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
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

        GameManager.Instance.mCurrentField.transform.position = new Vector3(mOffset.x, mOffset.y, mOffset.z);
        CameraSwitcher.UpdateCamera(GameManager.Instance.mCurrentField.transform);
        GameManager.Instance.mCurrentField.SetActive(true);
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
