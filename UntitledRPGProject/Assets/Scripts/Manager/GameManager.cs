using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        mCamera = Instantiate(Resources.Load<GameObject>("Prefabs/GameCamera"), transform.position, Quaternion.identity);
    }
    public GameState mGameState;
    public GameObject mCamera;
    public PlayerController mPlayer;
    public EnemyProwler mEnemyProwler;
    private List<Vector3> mOriginalFieldPos;
    public GameObject mCurrentField;
    private GameObject[] EnemyProwlers;

    public Dictionary<string, UnitDataStorage> mUnitData = new Dictionary<string, UnitDataStorage>();

    public static int s_ID = 0;
    public static int s_TotalExp = 0;
    public static int s_TotalGold = 0;
    public int TotalSoul = 0;
    //private int mCurrentLevel = 0;

    [SerializeField]
    private int mAmountofSoul = 20;
    [SerializeField]
    private float mWaitForRestart = 3.0f;

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
        mUnitData.Clear();
        Initialize();
    }

    private void Initialize()
    {
        mCurrentField = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Field"));
        mCurrentField.transform.parent = transform;
        mCurrentField.SetActive(false);
        s_ID = 0;
        s_TotalExp = 0;
        s_TotalGold = 0;
        TotalSoul = 0;
        //mCurrentLevel = 0;
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
                    OnBattleEnd();
                    //TODO: Display victory screen
                }
                break;
            case GameState.GameOver:
                {
                    GameOver();
                    //TODO: Display defeat screen
                }
                break;
            default:
                break;
        }

    }

    private void FinalizeBattle()
    {
        OnEnemyDeath(mEnemyProwler.id);
        ResetObjects();
    }

    private void GameOver()
    {
        onEnemyWin(Instance.mEnemyProwler.id, () =>
        {
            Instance.mEnemyProwler.isWin = true;
            mGameState = GameState.GamePlay;
        });
        UIManager.Instance.mOrderbar.GetComponent<OrderBar>().Clear();
        ResetObjects();
        // TODO: Gameover screen
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        TotalSoul += mAmountofSoul;
        UIManager.Instance.FadeInScreen();
        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.FadeInWord();
        UIManager.Instance.ChangeText("<color=red>" + UIManager.Instance.mTextForGameOver + "</color>");

        yield return new WaitForSeconds(mWaitForRestart);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        UIManager.Instance.FadeOutScreen();
        UIManager.Instance.FadeOutWord();

        s_ID = 0;

        s_TotalExp = 0;
        s_TotalGold = 0;
    }

    private void ResetObjects()
    {
        ActiveEnemyProwlers(true);
        BattleManager.Instance.StopAllCoroutines();
        for (int i = 0; i < Instance.mCurrentField.transform.Find("PlayerFields").childCount; ++i)
        {
            Instance.mCurrentField.transform.Find("PlayerFields").GetChild(i).transform.localPosition = mOriginalFieldPos[i];
        }

        for (int i = 0; i < Instance.mCurrentField.transform.Find("EnemyFields").childCount; ++i)
        {
            Instance.mCurrentField.transform.Find("EnemyFields").GetChild(i).transform.localPosition = mOriginalFieldPos[i + 4];
        }
        Instance.mCurrentField.SetActive(false);
        EnemyProwlers = null;
        mEnemyProwler = null;
    }

    public event Action onPlayerBattleStart;
    public event Action onPlayerBattleEnd;
    public event Action<int> onBattle;
    public void OnBattleStart(int id)
    {
        BattleManager.Instance.SetBattleField();
        onBattle?.Invoke(id); // Enemy preparation
        onPlayerBattleStart?.Invoke(); // Player preparation and camera switch

        EnemyProwlers = GameObject.FindGameObjectsWithTag("EnemyProwler");
        CameraSwitcher.SwitchCamera();
        BattleManager.Instance.Initialize();
        ActiveEnemyProwlers(false);
        mGameState = GameState.Busy;
    }

    public event Action<int> onEnemyDeath;
    public event Action<int, Action> onEnemyWin;

    public void OnEnemyWin(int id, Action action)
    {
        onEnemyWin?.Invoke(id, action);
        mEnemyProwler = null;
    }

    public void OnEnemyDeath(int id)
    {
        onEnemyDeath?.Invoke(id);
    }

    public void OnBattleEnd()
    {
        FinalizeBattle();
        UIManager.Instance.DisplayBattleInterface(false);
        onPlayerBattleEnd?.Invoke();
        mGameState = GameState.GamePlay;
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
