using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Load();
        }
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
                    {
                        if (Input.GetKeyDown(KeyCode.Escape))
                            Pause();
                    }
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
        if(mGameState == GameState.GamePlay)
        {
            mGameState = GameState.GamePause;
            UIManager.Instance.DisplayOptionScreen(true);
        }
        else
        {
            mGameState = GameState.GamePlay;
            UIManager.Instance.DisplayOptionScreen(false);
        }
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

    public void Save()
    {
        DataSaveSystem.SaveData(0);
        Debug.Log("Saved");
    }

    public void Load()
    {
        if (PlayerController.Instance == null)
            return;
        PlayerController.Instance.ResetAbility();
        PlayerController.Instance.ResetPlayerUnit();
        GameObject temp = new GameObject("Temp");
        Debug.Log(PlayerController.Instance.transform.childCount);
        for (int i = 0; i < PlayerController.Instance.transform.childCount; i++)
        {
            if (PlayerController.Instance.transform.GetChild(i).tag == "PlayerUnit")
            {
                PlayerController.Instance.transform.GetChild(i).transform.SetParent(temp.transform);
            }
        }
        PlayerController.Instance.mHeroes.Clear();
        Destroy(temp);
        characterExists.Clear();
        characterExists = new List<CharacterExist>(5)
    {
        new CharacterExist(NPCUnit.Vin, false),
        new CharacterExist(NPCUnit.Eleven, false),
        new CharacterExist(NPCUnit.Roger, false),
        new CharacterExist(NPCUnit.Victor, false),
        new CharacterExist(NPCUnit.Jimmy, true)
    };

        PlayerData data = DataSaveSystem.LoadData(0);
        PlayerController.Instance.mGold = data.mMoney;
        PlayerController.Instance.mSoul = data.mSoul;

        for (int i = 0; i < data.mUnlockedSkill_Nodes.Length; ++i)
        {
            string node = data.mUnlockedSkill_Nodes[i];
            SkillTreeManager.Instance.UnlockSkill(node);
        }

        bool found = false;

        for (int i = 0; i < data.mPossessedItems.Length; ++i)
        {
            string itemName = data.mPossessedItems[i];
            GameObject item = null;
            found = SearchItem(itemName, ref item);
            if(item != null)
            {
                item.GetComponent<Item>().isSold = true;
                PlayerController.Instance.mInventory.Add(item.GetComponent<Item>());
            }
            Debug.Log((found) ? $"{itemName} Found" : $"<color=yellow>Warning!</color> {itemName} doesn't exist!");
        }

        Hero[] CompanionNPC = GameObject.FindObjectsOfType<Hero>();
        foreach(var it in data.mPlayerUnitStatus)
        {
            AssignCharacter(it.Key.ToString());
            GameObject go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Units/Allys/" + it.Key.ToString()), PlayerController.Instance.transform.position, Quaternion.identity, PlayerController.Instance.transform);
            go.GetComponent<Unit>().ResetUnit();
            go.GetComponent<Unit>().mStatus = it.Value;
            go.SetActive(false);
            PlayerController.Instance.mHeroes.Add(go);
            foreach(var itr in CompanionNPC)
            {
                if(itr.mName.Contains(it.Key.ToString()))
                {
                    GameObject model = ResourceManager.GetResource<GameObject>("Prefabs/Effects/CompanionEffect");
                    GameObject effect = Instantiate(model, itr.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.Euler(model.transform.eulerAngles));
                    Destroy(effect, 1.5f);
                    Destroy(itr.gameObject);
                }
            }
        }

        foreach(var it in data.mPlayerUnitInventory)
        {
            string itemName = string.Empty;
            Unit unit = PlayerController.Instance.mHeroes.Find(n => n.GetComponent<Unit>().mSetting.Name == it.Key.ToString()).GetComponent<Unit>();
            InventroySystem system = unit.mInventroySystem;

            // Head
            itemName = it.Value.Head;
            if(itemName != "Null")
            {
                Armor head = PlayerController.Instance.mInventory.Get(itemName) as Armor;
                system.Equip(head);
            }            
            
            // Body
            itemName = it.Value.Body;
            if(itemName != "Null")
            {
                Armor body = PlayerController.Instance.mInventory.Get(itemName) as Armor;
                system.Equip(body);
            }           
            
            // Arm
            itemName = it.Value.Arm;
            if(itemName != "Null")
            {
                Armor arm = PlayerController.Instance.mInventory.Get(itemName) as Armor;
                system.Equip(arm);
            }           
            
            // Leg
            itemName = it.Value.Leg;
            if(itemName != "Null")
            {
                Armor leg = PlayerController.Instance.mInventory.Get(itemName) as Armor;
                system.Equip(leg);
            }            
            
            // Weapon
            itemName = it.Value.Weapon;
            if(itemName != "Null")
            {
                Weapon weapon = PlayerController.Instance.mInventory.Get(itemName) as Weapon;
                system.Equip(weapon);
            }
        }
    }

    private bool SearchItem(string itemName, ref GameObject it)
    {
        for (int x = 0; x < mArmorPool.Length; ++x)
        {
            var armor = mArmorPool[x];
            if (armor.GetComponent<Armor>().Info.mName == itemName)
            {
                it = Instantiate(armor, PlayerController.Instance.transform.Find("Bag"));
                return true;
            }
        }
        for (int x = 0; x < mWeaponPool.Length; ++x)
        {
            var weapon = mWeaponPool[x];
            if (weapon.GetComponent<Weapon>().Info.mName == itemName)
            {
                it = Instantiate(weapon, PlayerController.Instance.transform.Find("Bag"));
                return true;
            }
        }
        return false;
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
