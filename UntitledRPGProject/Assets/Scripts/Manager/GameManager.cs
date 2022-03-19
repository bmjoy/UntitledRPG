using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager mInstance;
    public static GameManager Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    public GameState mGameState;
    public PlayerController mPlayer;
    public EnemyProwler mEnemyProwler;
    [SerializeField]
    private GameObject mField;
    public GameObject mCurrentField;
    public GameObject[] EnemyProwlers;
    private void Start()
    {
        mCurrentField = GameObject.Instantiate(Instance.mField, Vector3.zero, Quaternion.identity);
        mCurrentField.SetActive(false);

        EnemyProwlers = GameObject.FindGameObjectsWithTag("EnemyProwler");
    }
    private void Update()
    {
        UpdateGameState();
    }
    private void UpdateGameState()
    {
        switch (mGameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Time.timeScale = 0.0f;
                        mGameState = GameState.GamePause;
                    }
                }
                break;
            case GameState.GamePause:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Time.timeScale = 1.0f;
                        mGameState = GameState.GamePlay;
                    }
                }
                break;
            case GameState.Battle:
                {
                    if(!BattleManager.Instance.isBattle)
                    {
                        CameraSwitcher.SwitchCamera();
                        BattleManager.Instance.Initialize();
                    }
                }
                break;
            case GameState.Victory:
                {
                    FinalizeBattle();
                    mGameState = GameState.GamePlay;
                    //TODO: Display victory screen
                }
                break;
            case GameState.GameOver:
                {
                    FinalizeBattle();
                    //TODO: Display defeat screen
                }
                break;
            default:
                break;
        }

    }

    private void FinalizeBattle()
    {
        BattleManager.Instance.StopAllCoroutines();
        OnEnemyDeath(mEnemyProwler.id);
        for (int i = 0; i < EnemyProwlers.Length; i++)
        {
            if(EnemyProwlers[i] != null)
            {
                if (EnemyProwlers[i].GetComponent<EnemyProwler>().id == mEnemyProwler.id)
                    continue;
                EnemyProwlers[i].SetActive(true);
            }
        }
        Instance.mCurrentField.SetActive(false);
        OnBattleEnd();
    }

    public event Action onPlayerBattleStart;
    public event Action onPlayerBattleEnd;
    public event Action<int> onBattle;
    public void OnBattleStart(int id)
    {
        onPlayerBattleStart?.Invoke(); // Player preparation and camera switch
        onBattle?.Invoke(id); // Enemy preparation
        mGameState = GameState.Battle;
    }

    public event Action<int> onEnemyDeath;
    public void OnEnemyDeath(int id)
    {
        onEnemyDeath?.Invoke(id);
    }

    public void OnBattleEnd()
    {
        UIManager.Instance.DisplayBattleInterface(false);
        onPlayerBattleEnd?.Invoke();
    }
}
