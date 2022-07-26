using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager mInstance;
    public static UIManager Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("Key Setting")]
    public KeyCode mYesKeyCode;
    public KeyCode mNoKeyCode;
    public KeyCode mExitKeyCode;
    [Header("UI Components")]
    public UIStorage mStorage;
    public Canvas mCanvas;
    public Canvas mAdditionalCanvas;
    public Camera mUICamera;

    // ----- Animator -----
    private Animator mVictoryTransitionAnimator;
    private Animator mTransitionAnimator;
    private Animator mTextAnimator;
    private Animator mFadeAnimator;

    private List<BigHealthBar> mHealthBarList = new List<BigHealthBar>();
    private BossHealthBar mBossHealthBar;


    // ----- Screen -----
    public OrderBar mOrderBar;
    public InventoryUI mInventoryUI;
    public OptionScreen mOptionScreenUI;
    public VictoryScreen mVictoryScreen;
    public MerchantScreen mMerchantScreen;
    public SkillTreeScreen mSkillTreeScreen;
    private bool switchOfInventory = false;
    [HideInInspector]
    public bool IsOpenScreen = false;
    private float mTime = 0.0f;
    private float mCoolTime = 1.5f;

    void Start()
    {
        mCanvas = transform.Find("Canvas").GetComponent<Canvas>();
        mAdditionalCanvas = transform.Find("AdditionalCanvas").GetComponent<Canvas>();
        mUICamera = transform.Find("UICamera").GetComponent<Camera>();
        mCanvas.overrideSorting = true;
        mHealthBarList = mCanvas.transform.Find("HealthBarInBattleGroundGroup").GetComponentsInChildren<BigHealthBar>().ToList();
        mBossHealthBar = mCanvas.transform.Find("BossBorader").Find("HealthBarInBattleGround").GetComponent<BossHealthBar>();

        GameManager.Instance.onFadeGameOverScreenEvent += FadeInScreen;
        GameManager.Instance.onFadeGameOverScreenEvent += FadeInWord;

        GameManager.Instance.onGameOverToReset += FadeOutScreen;
        GameManager.Instance.onGameOverToReset += FadeOutWord;
        GameManager.Instance.onGameOverToReset += ResetUI;

        BattleManager.Instance.onEnqueuingOrderEvent += BattleStart;
        BattleManager.Instance.onFinishOrderEvent += BattleEnd;
        mStorage.Initialize();
        mInventoryUI = mCanvas.transform.Find("Inventory").GetComponent<InventoryUI>();
        mVictoryScreen = mCanvas.transform.Find("VictoryScreen").GetComponent<VictoryScreen>();
        mMerchantScreen = mCanvas.transform.Find("MerchantBox").GetComponent<MerchantScreen>();
        mOptionScreenUI = mCanvas.transform.Find("OptionScreen").GetComponent<OptionScreen>();
        mSkillTreeScreen = mCanvas.transform.Find("SkillTreeScreen").GetComponent<SkillTreeScreen>();
        mSkillTreeScreen.Initialize();
        mTransitionAnimator = mStorage.mScreenTransition.GetComponent<Animator>();
        mTextAnimator = mStorage.mBasicText.GetComponent<Animator>();
        mFadeAnimator = mStorage.mFadeScreen.GetComponent<Animator>();
        mVictoryTransitionAnimator = mStorage.mVictoryScreenTransition.GetComponent<Animator>();
        mOrderBar = mStorage.mOrderbar.GetComponent<OrderBar>();
        mInventoryUI.Initialize();
        mVictoryScreen.Initialize(this);
        DisplayBattleInterface(false);
        DisplayText(false);
        DisplayDialogueBox(false);
        DisplayInventory(false);
        DisplaySkillTreeScreen(false);
    }

    public void DisplayInventory(bool active)
    {
        mInventoryUI.Active(active);
        IsOpenScreen = active;
        if(PlayerController.Instance)
            PlayerController.Instance.mInteractSystem.IsInteracting = active;
    }
    public void DisplaySkillTreeScreen(bool active)
    {
        mSkillTreeScreen.Active(active);
        IsOpenScreen = active;
    }
    private void Update()
    {
        if (PlayerController.Instance && PlayerController.Instance.onBattle)
            return;

        if (GameManager.Instance.IsCinematicEvent)
            return;

        if(Input.GetKeyDown(KeyCode.I) && mTime <= 0.0f && GameManager.mGameState == GameState.GamePlay)
        {
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mOpenInventorySFX);
            DisplayInventory();
            mTime += mCoolTime;
        }
        if (mTime >= 0.0f)
            mTime -= Time.deltaTime;
    }

    public void DisplayInventory()
    {
        DisplayInventory(!switchOfInventory);
        DisplayMiniMap(switchOfInventory);
        switchOfInventory = !switchOfInventory;
    }

    public void DisplayMiniMap(bool active)
    {
        mStorage.mMinimap.SetActive(active);
    }

    public IEnumerator VictoryTransition()
    {
        mStorage.mVictoryScreenTransition.SetActive(true);
        mVictoryTransitionAnimator.Play("Expand");
        yield return new WaitForSeconds(3.0f);
        mStorage.mVictoryScreenTransition.SetActive(false);
    }

    public static void ResetUI()
    {
        Instance.DisplayBattleInterface(false);
        Instance.DisplayDialogueBox(false);
    }

    public void BattleStart()
    {
        FadeInScreen(() => { StopFade(); });
        mStorage.mScreenTransition.SetActive(true);
        mTransitionAnimator.Play("Expand");
    }

    public void BattleEnd()
    {
        for (int i = 0; i < mHealthBarList.Count; i++)
            mHealthBarList[i].Active(false);
        mOrderBar.Clear();
        mOrderBar.gameObject.SetActive(false);
        mStorage.mScreenTransition.SetActive(false);
    }

    public void DisplayBattleInterface(bool display)
    {
        mStorage.mBattleUI.SetActive(display);
    }

    public void DisplayText(bool display)
    {
        mStorage.mBasicText.SetActive(display);
    }    
    public void DisplayOrderBarText(bool display)
    {
        mOrderBar.mText.gameObject.SetActive(display);
        if (display)
        {
            mOrderBar.mText.transform.localPosition = new Vector3(Screen.width / 3.0f, 0, 0);
            ChangeOrderBarText("");
        }
    }

    public void DisplayOptionScreen(bool display)
    {
        mOptionScreenUI.Active(display);
    }

    public void DisplayDialogueBox(bool display)
    {
        IsOpenScreen = display;
        if (display)
        {
            mStorage.mDialogueText.gameObject.SetActive(display);
            mStorage.mDialogueBox.SetActive(display);
        }
        else
            StartCoroutine(EndOfDialogueBox());
    }

    private IEnumerator EndOfDialogueBox()
    {
        if(mStorage.mDialogueBox.activeSelf)
            mStorage.mDialogueBox.GetComponent<Animator>().Play("Outro");
        yield return new WaitForSeconds(1.0f);
        mStorage.mDialogueText.gameObject.SetActive(false);
        mStorage.mDialogueBox.SetActive(false);
    }

    public void DisplayHealthBar(bool display)
    {
        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            mHealthBarList[i].gameObject.SetActive(display);
            mHealthBarList[i].Active(display);
            if (display)
            {
                var unit = PlayerController.Instance.mHeroes[i].GetComponent<Player>();
                mHealthBarList[i].Initialize(unit); 
                unit.mMyHealthBar = mHealthBarList[i];
            }

        }

        if(GameManager.Instance.mEnemyProwler)
        {
            for (int i = 0; i < GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Count; ++i)
            {
                if (GameManager.Instance.mEnemyProwler.mEnemySpawnGroup[i].GetComponent<Unit>().GetType() == typeof(Boss))
                {
                    mBossHealthBar.gameObject.SetActive(display);
                    mBossHealthBar.Active(display);
                    mBossHealthBar.Initialize(
        BattleManager.Instance.mUnits[i].GetComponent<Unit>().mStatus.mHealth,
        BattleManager.Instance.mUnits[i].GetComponent<Unit>().mStatus.mMaxHealth,
        0.0f, 0.0f);
                    GameManager.Instance.mEnemyProwler.mEnemySpawnGroup[i].GetComponent<Boss>().mMyHealthBar = mBossHealthBar;
                }
            }
        }
        if(!display)
        {
            mBossHealthBar.gameObject.SetActive(false);
            mBossHealthBar.Active(false);
        }
    }

    public IEnumerator Celebration(bool isLevelUP)
    {
        if (isLevelUP)
        {
            GameObject fireworksTop = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/CelebrationEffect2"), mAdditionalCanvas.transform.localPosition + new Vector3(0.0f, 25.0f, 0.0f), Quaternion.identity, mAdditionalCanvas.transform);
            float random = 0.0f;
            while (true)
            {
                GameObject fireworks = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/CelebrationEffect1"), mAdditionalCanvas.transform.localPosition + new Vector3(UnityEngine.Random.Range(-50.0f, 50.0f), UnityEngine.Random.Range(-50.0f, 50.0f), 0.0f), Quaternion.identity, mAdditionalCanvas.transform);
                Destroy(fireworks, 3.0f);
                random = UnityEngine.Random.Range(0.25f, 0.7f);
                yield return new WaitForSeconds(random);
                if (BattleManager.status == BattleManager.GameStatus.None)
                    break;
            }
            fireworksTop.GetComponent<ParticleSystem>().Stop();
            Destroy(fireworksTop, 5.0f);
            yield return null;
        }
        else
            yield return null;
    }

    public void ChangeHoverTip(string text, string action)
    {
        if (action == "Skill")
            mStorage.mSkillDescription.GetComponent<HoverTip>().mTipToShow = text;
        else if (action == "Attack")
            mStorage.mAttackDescription.GetComponent<HoverTip>().mTipToShow = text;
        else if (action == "Defend")
            mStorage.mDefendDescription.GetComponent<HoverTip>().mTipToShow = text;
        else
            Debug.LogWarning("<color=yellow>Warning! " + "</color>" + action + " doesn't exist!");
    }

    public void ChangeText(string text)
    {
        mStorage.mBasicText.GetComponent<TextMeshProUGUI>().text = text;
    }    
    public void ChangeOrderBarText(string text)
    {
        mOrderBar.ChangeText(text);
    }

    public void ChangeDialogueText(string text)
    {
        mStorage.mDialogueText.text = text;
    }

    public void StopFade()
    {
        StartCoroutine(Stop());
    }

    private IEnumerator Stop()
    {
        yield return new WaitForSeconds(1.0f);
        FadeOutScreen();
        yield return new WaitForSeconds(0.5f);

        DisplayHealthBar(true);
        mStorage.mOrderbar.gameObject.SetActive(true);
    }

    public void FadeInScreen(Action action = null)
    {
        mStorage.mFadeScreen.SetActive(true);
        mFadeAnimator.SetBool("FadeIn",true);
        mFadeAnimator.SetBool("FadeOut", false);
        action?.Invoke();
    }

    public void FadeInWord(Action action = null)
    {
        DisplayText(true);
        mTextAnimator.SetBool("FadeIn", true);
        mTextAnimator.SetBool("FadeOut", false);
        action?.Invoke();
    }

    public void FadeOutScreen()
    {
        mFadeAnimator.SetBool("FadeOut", true);
        mFadeAnimator.SetBool("FadeIn", false);
    }

    public void FadeOutWord()
    {
        mTextAnimator.SetBool("FadeIn", false);
        mTextAnimator.SetBool("FadeOut", true);
    }
    public void DisplayMoneyBoxInDialogue(bool action, int cost = 0)
    {
        StopCoroutine("DisplayMoneyBoxInTime");
        mStorage.mCurrentMoney.SetActive(action);
        mStorage.mRequireMoneyBox.SetActive(action);
        if(action)
        {
            mStorage.mRequireMoneyBox.transform.Find("Value").GetComponent<TextMeshProUGUI>().text
                = cost.ToString();
            mStorage.mCurrentMoney.GetComponent<Animator>().Play("Idle");
        }
    }

    public IEnumerator DisplayMoneyBoxInTime(float t)
    {
        mStorage.mCurrentMoney.SetActive(true);
        mStorage.mCurrentMoney.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(t);
        mStorage.mCurrentMoney.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        mStorage.mCurrentMoney.SetActive(false);
    }

    public void DisplaySupportKey(bool E = true, bool R = false, bool ESC = false)
    {
        mStorage.mSupportKey.SetActive(E);
        mStorage.mSupportKey.transform.Find("ESC Key").gameObject.SetActive(ESC);
        mStorage.mSupportKey.transform.Find("E Key").gameObject.SetActive(E);
        mStorage.mSupportKey.transform.Find("R Key").gameObject.SetActive(R);
    }

    public void ChangeSupportText(string[] strings)
    {
        if (strings[2] != string.Empty)
            mStorage.mSupportKey.transform.Find("ESC Key").Find("Text").GetComponent<TextMeshProUGUI>().text = strings[2];
        if (strings[0] != string.Empty)
            mStorage.mSupportKey.transform.Find("E Key").Find("Text").GetComponent<TextMeshProUGUI>().text = strings[0];
        if (strings[1] != string.Empty)
            mStorage.mSupportKey.transform.Find("R Key").Find("Text").GetComponent<TextMeshProUGUI>().text = strings[1];
    }

    public void DisplayTutorialIcon(string icon, bool here = false)
    {
        switch(icon)
        {
            case "Move":
                mStorage.mTutorialHereIcon.SetActive(here);
                mStorage.mTutorialItemIcon.SetActive(false);
                mStorage.mTutorialWASDIcon.SetActive(true);
                mStorage.mTutorialInteractIcon.SetActive(false);
                break;
            case "Interact":
                mStorage.mTutorialHereIcon.SetActive(here);
                mStorage.mTutorialItemIcon.SetActive(false);
                mStorage.mTutorialWASDIcon.SetActive(false);
                mStorage.mTutorialInteractIcon.SetActive(true);
                break;
            case "Item":
                mStorage.mTutorialItemIcon.SetActive(true);
                mStorage.mTutorialWASDIcon.SetActive(false);
                mStorage.mTutorialHereIcon.SetActive(here);
                mStorage.mTutorialInteractIcon.SetActive(false);
                break;
            case "None":
                mStorage.mTutorialItemIcon.SetActive(false);
                mStorage.mTutorialWASDIcon.SetActive(false);
                mStorage.mTutorialHereIcon.SetActive(here);
                mStorage.mTutorialInteractIcon.SetActive(false);
                break;
            default:
                Debug.LogWarning($"<color=yellow>Warning!</color> The name of {icon} is not vaild!");
                break;
        }
    }
}
