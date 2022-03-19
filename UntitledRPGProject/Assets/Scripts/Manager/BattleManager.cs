using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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
    private bool isActed = false;

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
        mUnits.Sort((a, b) => (b.GetComponent<Unit>().mAgility.CompareTo(
            a.GetComponent<Unit>().mAgility)));
        for (int i = 0; i < mUnits.Count; i++)
        {
            mUnits[i].GetComponent<Unit>().mOrder = Order.Standby;
            mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
        }

        StartCoroutine(UpdateBettle());
    }

    private IEnumerator UpdateBettle()
    {
        mCurrentUnit = null;
        while (isBattle)
        {
            // It's a unit's turn
            if (mCurrentUnit == null && mOrders.Count != 0)
            {
                UIManager.Instance.DisplayBattleInterface(false);
                yield return new WaitForSeconds(0.5f);
                mCurrentUnit = mOrders.Dequeue();
                if(mCurrentUnit.isDied)
                {
                    mCurrentUnit = null;
                    yield return null;
                }
                else
                {
                    mCurrentUnit.isDefend = false;
                    Debug.Log(mCurrentUnit.ToString());
                    mCurrentUnit.isPicked = true;
                    mCurrentUnit.mOrder = Order.Standby;
                    if (mCurrentUnit.mFlag == Flag.Player)
                        UIManager.Instance.DisplayBattleInterface(true);
                }
            }

            if (BattleResult() == true)
                break;

            if (mCurrentUnit != null && mCurrentUnit.mOrder == Order.TurnEnd)
            {
                mCurrentUnit = null;
            }

            if (mOrders.Count == 0 && mCurrentUnit == null)
            {
                yield return new WaitForSeconds(2.0f);
                for (int i = 0; i < mUnits.Count; i++)
                {
                    if (!mUnits[i].GetComponent<Unit>().isDied)
                        mOrders.Enqueue(mUnits[i].GetComponent<Unit>());
                }
            }

            yield return null;
        }
        CameraSwitcher.SwitchCamera();
        yield return new WaitForSeconds(1.0f);
        if (isWin)
            GameManager.Instance.mGameState = GameState.Victory;
        else
            GameManager.Instance.mGameState = GameState.GameOver;
        isBattle = false;
        mCurrentUnit = null;
        yield return null;
    }

    private bool BattleResult()
    {
        if (GameManager.Instance.mEnemyProwler.EnemySpawnGroup.TrueForAll(t => t.GetComponent<Unit>().isDied))
        {
            isWin = true;
            return true;
        }
        else if (GameManager.Instance.mPlayer.mHeroes.TrueForAll(t => t.GetComponent<Unit>().isDied))
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
        if(mCurrentUnit.Unit_Setting.mTarget == null)
        {
            Debug.Log("No Target!");
            return;
        }    
        if (!isActed && mCurrentUnit.isPicked)
        {
            Debug.Log("Attack");
            StartCoroutine(mCurrentUnit.AttackAction(mCurrentUnit.Unit_Setting.mTarget, DamageType.Physical));
            isActed = true;
            StartCoroutine(TrunFinished());
        }
    }

    public void Defend()
    {
        if(!isActed && mCurrentUnit.isPicked)
        {
            mCurrentUnit.isDefend = true;
            isActed = true;
            StartCoroutine(TrunFinished());
        }
    }

    private IEnumerator TrunFinished()
    {
        yield return new WaitForSeconds(0.5f);
        mCurrentUnit.TurnEnded();
        isActed = false;
    }
}
