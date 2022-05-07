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
        Debug.Log("Hi GameManager");
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    public GameState mGameState;
    public PlayerController mPlayer;
    public EnemyProwler mEnemyProwler;
    private List<Vector3> mOriginalFieldPos;
    public GameObject mCurrentField;
    private GameObject[] EnemyProwlers;

    public static int s_ID = 0;
    public static int s_TotalExp = 0;
    public static int s_TotalGold = 0;

    private void Start()
    {
        mOriginalFieldPos = new List<Vector3>()
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

        mPlayer = GameObject.Find("Player").GetComponent<PlayerController>();
        if (GameObject.Find("GameCamera") == null)
        {
            GameObject source = Instantiate(Resources.Load<GameObject>("Prefabs/GameCamera"), transform.position, Quaternion.identity);
            source.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = mPlayer.transform;
            source.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = mPlayer.transform;
        }
        mCurrentField = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Field"));
        mCurrentField.transform.parent = transform;
        mCurrentField.SetActive(false);
        s_ID = 0;
        s_TotalExp = 0;
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
            case GameState.Busy:
                {
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

        ActiveEnemyProwlers(true);
        EnemyProwlers = null;
        for (int i = 0; i < Instance.mCurrentField.transform.Find("PlayerFields").childCount; ++i)
        {
            Instance.mCurrentField.transform.Find("PlayerFields").GetChild(i).transform.localPosition = mOriginalFieldPos[i];
        }

        for (int i = 0; i < Instance.mCurrentField.transform.Find("EnemyFields").childCount; ++i)
        {
            Instance.mCurrentField.transform.Find("EnemyFields").GetChild(i).transform.localPosition = mOriginalFieldPos[i + 4];
        }
        Instance.mCurrentField.SetActive(false);

        mEnemyProwler = null;
        OnBattleEnd();
    }

    public event Action onPlayerBattleStart;
    public event Action onPlayerBattleEnd;
    public event Action<int> onBattle;
    public void OnBattleStart(int id)
    {
        onPlayerBattleStart?.Invoke(); // Player preparation and camera switch
        onBattle?.Invoke(id); // Enemy preparation
        EnemyProwlers = GameObject.FindGameObjectsWithTag("EnemyProwler");
        CameraSwitcher.SwitchCamera();
        ActiveEnemyProwlers(false);
        BattleManager.Instance.Initialize();
        mGameState = GameState.Busy;
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

    private void ActiveEnemyProwlers(bool active)
    {
        for (int i = 0; i < Instance.EnemyProwlers.Length; ++i)
        {
            if (Instance.EnemyProwlers[i] != null)
            {
                if (Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().id == Instance.mEnemyProwler.id)
                    continue;
                Instance.EnemyProwlers[i].SetActive(active);
                Instance.EnemyProwlers[i].GetComponent<BoxCollider>().enabled = active;
            }
        }
    }
}
