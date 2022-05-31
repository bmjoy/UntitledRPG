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
        mUnitData.Clear();
        mCamera = Instantiate(Resources.Load<GameObject>("Prefabs/GameCamera"), transform.position, Quaternion.identity);
    }

    public GameState mGameState;
    private GameObject[] EnemyProwlers;
    private GameObject[] NPCProwlers;
    public Dictionary<string, UnitDataStorage> mUnitData = new Dictionary<string, UnitDataStorage>();
    public GameObject[] mArmorPool;
    public GameObject[] mWeaponPool;
    public static int s_ID = 0;
    public static int s_TotalExp = 0;
    public static int s_TotalGold = 0;
    public static int s_TotalSoul = 0;
    public int _ItemID = 0;
    [HideInInspector]
    public GameObject mCamera;
    [HideInInspector]
    public EnemyProwler mEnemyProwler;
    [HideInInspector]
    public int mCurrentLevel = 0;

    public int mRequiredEXP = 500;
    public int mAmountofSoul = 20;
    [SerializeField]
    private float mWaitForRestart = 3.0f;

    private void Start()
    {
        mUnitData.Clear();
        Initialize();
    }

    private void Initialize()
    {
        s_ID = 0;
        s_TotalExp = 0;
        s_TotalGold = 0;
        _ItemID = 0;
        s_TotalSoul = 0;
        mArmorPool = Resources.LoadAll<GameObject>("Prefabs/Items/Equipments/Armors/");
        mWeaponPool = Resources.LoadAll<GameObject>("Prefabs/Items/Equipments/Weapons/");
    }

    private void Update()
    {
        UpdateGameState();
    }
    private void UpdateGameState()
    {
        switch (mGameState)
        {
            case GameState.MainMenu: break;
            case GameState.GamePlay:
            case GameState.GamePause:Pause(); break;
            case GameState.Busy:break;
            case GameState.Victory: OnBattleEnd(); break;
            case GameState.GameOver: GameOver(); break;
        }

    }

    private void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            mGameState = (mGameState == GameState.GamePlay) ? GameState.GamePause : GameState.GamePlay;
        Time.timeScale = (mGameState == GameState.GamePause) ? 0.0f : 1.0f;
    }

    private void GameOver()
    {
        CameraSwitcher.SwitchCamera();
        onEnemyWin(Instance.mEnemyProwler.id, () =>
        {
            Instance.mEnemyProwler.isWin = true;
            mGameState = GameState.GamePlay;
        });
        UIManager.Instance.mOrderbar.GetComponent<OrderBar>().Clear();
        CameraSwitcher.Instance.mBloom.intensity.value = 0.0f;
        ResetObjects();
        PlayerController.Instance.IsDied = true;
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        onFadeGameOverScreenEvent?.Invoke(null);
        UIManager.Instance.ChangeText("<color=red>" + UIManager.Instance.mTextForGameOver + "</color>");
        yield return new WaitForSeconds(mWaitForRestart);
        onGameOverToReset?.Invoke();
        s_ID = 0;
        s_TotalExp = 0;
        s_TotalGold = 0;
    }

    private void ResetObjects()
    {
        ActiveAllProwlers(true);
        BattleManager.Instance.StopAllCoroutines();
        BattleManager.Instance.ResetField();
        Destroy(mEnemyProwler.gameObject);
        EnemyProwlers = null;
        mEnemyProwler = null;
    }

    public event Action onPlayerBattleStart;
    public event Action onPlayerBattleEnd;
    public event Action<int> onBattle;
    public event Action<Action> onFadeGameOverScreenEvent;
    public event Action onGameOverToReset;
    public event Action<int, Action> onEnemyWin;
    public void OnBattleStart(int id)
    {
        BattleManager.Instance.SetBattleField();
        onBattle?.Invoke(id); // Enemy preparation
        onPlayerBattleStart?.Invoke(); // Player preparation and camera switch
        EnemyProwlers = GameObject.FindGameObjectsWithTag("EnemyProwler");
        NPCProwlers = GameObject.FindGameObjectsWithTag("Neutral");
        ActiveAllProwlers(false);
        mGameState = GameState.Busy;
    }


    public void OnEnemyWin(int id, Action action)
    {
        onEnemyWin?.Invoke(id, action);
        mEnemyProwler = null;
    }

    public void OnBattleEnd()
    {
        CameraSwitcher.SwitchCamera();
        ResetObjects();
        UIManager.Instance.DisplayBattleInterface(false);
        onPlayerBattleEnd?.Invoke();
        mGameState = GameState.GamePlay;
    }

    private void ActiveAllProwlers(bool active)
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

        for (int i = 0; i < Instance.NPCProwlers.Length; i++)
        {
            if(Instance.NPCProwlers[i] != null)
            {
                Instance.NPCProwlers[i].SetActive(active);
            }
        }
    }
}
