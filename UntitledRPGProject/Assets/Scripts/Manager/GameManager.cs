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
        mCamera = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/GameCamera"), transform.position, Quaternion.identity);
    }

    public static GameState mGameState = GameState.MainMenu;
    private GameObject[] EnemyProwlers;
    private GameObject[] NPCProwlers;
   
    public GameObject[] mArmorPool;
    public GameObject[] mWeaponPool;
    public static int s_ID = 0;
    public static int s_TotalExp = 0;
    public static int s_TotalGold = 0;
    public static int s_TotalSoul = 0;
    [HideInInspector]
    public GameObject mCamera;
    [HideInInspector]
    public EnemyProwler mEnemyProwler;
    public int mRequiredEXP = 100;
    public int mAmountofSoul = 20;
    public int mFinisherChance = 30;
    [SerializeField]
    private float mWaitForRestart = 3.0f;

    public static List<CharacterExist> characterExists;

    public bool IsCinematicEvent = false;
    private void Start()
    {
        Initialize();
        characterExists = new List<CharacterExist>(5)
    {
        new CharacterExist(NPCUnit.Vin, false),
        new CharacterExist(NPCUnit.Eleven, false),
        new CharacterExist(NPCUnit.Roger, false),
        new CharacterExist(NPCUnit.Victor, false),
        new CharacterExist(NPCUnit.Jimmy, true)
    };
    }

    private void Initialize()
    {
        s_ID = 0;
        s_TotalExp = 0;
        s_TotalGold = 0;
        s_TotalSoul = 0;
        mArmorPool = Resources.LoadAll<GameObject>("Prefabs/Items/Equipments/Armors/");
        mWeaponPool = Resources.LoadAll<GameObject>("Prefabs/Items/Equipments/Weapons/");
    }

    private void Update()
    {
        UpdateGameState();
    }

    public void AssignCharacter(string unit)
    {
        characterExists.Find(x => x.mUnit.ToString() == unit).isExist = true;
    }

    public bool IsExist(string unit)
    {
        return characterExists.Find(x => x.mUnit.ToString() == unit).isExist;
    }

    private void UpdateGameState()
    {
        switch (mGameState)
        {
            case GameState.MainMenu: break;
            case GameState.Initialize:
                {
                    AudioManager.Instance.mAudioStorage.ChangeMusic("Background");
                    mGameState = GameState.GamePlay;
                    UIManager.Instance.mUICamera.fieldOfView = 40.0f;
                    UIManager.Instance.DisplayMiniMap(true);
                }
                break;
            case GameState.GamePlay:
            case GameState.GamePause:
                {
                    if(UIManager.Instance && !UIManager.Instance.IsOpenScreen)
                        Pause();
                }
                break;
            case GameState.Victory: OnBattleEnd(); break;
            case GameState.GameOver:
                {
                    GameOver();
                    break;
                }
        }
    }

    public void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            mGameState = (mGameState == GameState.GamePlay) ? GameState.GamePause : GameState.GamePlay;
        if (mGameState == GameState.GamePause)
            UIManager.Instance.DisplayOptionScreen(true);
        else
            UIManager.Instance.DisplayOptionScreen(false);
        Time.timeScale = (mGameState == GameState.GamePause) ? 0.0f : 1.0f;

    }

    public void Exit()
    {
        Application.Quit();
    }

    public static void GameReset()
    {
        Instance.Initialize();
        if (UIManager.Instance.mInventoryUI.gameObject.activeSelf)
            UIManager.Instance.mInventoryUI.Active(false);
        mGameState = GameState.MainMenu;
        characterExists.Clear();
        characterExists = new List<CharacterExist>(5)
    {
        new CharacterExist(NPCUnit.Vin, false),
        new CharacterExist(NPCUnit.Eleven, false),
        new CharacterExist(NPCUnit.Roger, false),
        new CharacterExist(NPCUnit.Victor, false),
        new CharacterExist(NPCUnit.Jimmy, true)
    };
    }

    private void GameOver()
    {
        // TODO: Gameover music
        BattleManager.status = BattleManager.GameStatus.Finish;
        UIManager.Instance.BattleEnd();
        UIManager.Instance.DisplayMiniMap(false);
        AudioManager.Instance.mAudioStorage.ChangeMusic("Defeat");
        AudioManager.Instance.musicSource.loop = false;

        if(mEnemyProwler != null)
        {
            CameraSwitcher.SwitchCamera();
            onEnemyWin(Instance.mEnemyProwler.id, () =>
            {
                Instance.mEnemyProwler.isWin = true;
                mGameState = GameState.GamePlay;
            });
            UIManager.Instance.mStorage.mOrderbar.GetComponent<OrderBar>().Clear();
        }
        if(UIManager.Instance.mInventoryUI.gameObject.activeSelf)
            UIManager.Instance.mInventoryUI.Active(false);
        
        ResetObjects();
        PlayerController.Instance.IsDied = true;
        mGameState = GameState.GamePlay;
        characterExists = new List<CharacterExist>(5)
    {
        new CharacterExist(NPCUnit.Vin, false),
        new CharacterExist(NPCUnit.Eleven, false),
        new CharacterExist(NPCUnit.Roger, false),
        new CharacterExist(NPCUnit.Victor, false),
        new CharacterExist(NPCUnit.Jimmy, true)
    };
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        onFadeGameOverScreenEvent?.Invoke(null);
        UIManager.Instance.ChangeText("<color=red>" + UIManager.Instance.mStorage.mTextForGameOver + "</color>");
        yield return new WaitForSeconds(mWaitForRestart);
        onGameOverToReset?.Invoke();
        s_ID = 0;
        s_TotalExp = 0;
        s_TotalGold = 0;
        AudioManager.Instance.mAudioStorage.ChangeMusic("Background");
        AudioManager.Instance.musicSource.loop = true;
        Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
            Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = PlayerController.Instance.transform;
        UIManager.Instance.DisplayMiniMap(true);
    }

    private void ResetObjects()
    {
        if(EnemyProwlers != null)
            ActiveAllProwlers(true);
        BattleManager.Instance.StopAllCoroutines();
        if(BattleManager.Instance.mCurrentField.activeSelf)
            BattleManager.Instance.ResetField();
        if(mEnemyProwler)
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
        UIManager.Instance.DisplayMiniMap(false);
        ActiveAllProwlers(false);
    }


    public void OnEnemyWin(int id, Action action)
    {
        onEnemyWin?.Invoke(id, action);
        mEnemyProwler = null;
    }

    public void OnBattleEnd()
    {
        AudioManager.Instance.mAudioStorage.ChangeMusic("Background");
        AudioManager.Instance.musicSource.loop = true;
        UIManager.Instance.DisplayMiniMap(true);
        CameraSwitcher.SwitchCamera();
        ResetObjects();
        UIManager.Instance.DisplayBattleInterface(false);
        onPlayerBattleEnd?.Invoke();
        mGameState = GameState.GamePlay;
    }

    public void ActiveAllProwlers(bool active)
    {
        for (int i = 0; i < Instance.EnemyProwlers.Length; ++i)
        {
            if (Instance.EnemyProwlers[i] != null)
            {
                Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().mExclamation.SetActive(false);
                Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().mParticles.SetActive(false);
                if (Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().id == Instance.mEnemyProwler.id)
                    continue;
                Instance.EnemyProwlers[i].GetComponent<EnemyProwler>().ChangeBehavior("Stop");
                Instance.EnemyProwlers[i].SetActive(active);
                Instance.EnemyProwlers[i].GetComponent<BoxCollider>().enabled = active;
            }
        }

        for (int i = 0; i < Instance.NPCProwlers.Length; i++)
        {
            if(Instance.NPCProwlers[i] != null)
                Instance.NPCProwlers[i].SetActive(active);
        }
    }
}
