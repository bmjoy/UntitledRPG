using System;
using System.Collections;
using System.Collections.Generic;
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
    [TextArea]
    public string mTextForTarget;
    [TextArea]
    public string mTextForAccpet;
    [TextArea]
    public string mTextForGameOver = "Wasted!";

    public Canvas mCanvas;
    public GameObject mBattleUI;
    public GameObject mSkillDescription;
    public GameObject mAttackDescription;
    public GameObject mDefendDescription;
    public GameObject mSkillUseCheck;
    public GameObject mBasicText;
    public GameObject mFadeScreen;
    public GameObject mDialogueBox;
    public GameObject mScreenTransition;
    private Animator mTransitionAnimator;      
    public GameObject mVictoryScreenTransition;
    private Animator mVictoryTransitionAnimator;

    private List<BigHealthBar> mHealthBarList = new List<BigHealthBar>();

    private TextMeshProUGUI mDialogueText;

    private Button mYesButton;
    private Button mNoButton;
    private Button mExitButton;
    private Image mEKeyButton;

    public GameObject mOrderbar;
    private BossHealthBar mBossHealthBar;

    public InventoryUI mInventoryUI;
    public OptionScreen mOptionScreenUI;
    public VictoryScreen mVictoryScreen;
    public MerchantScreen mMerchantScreen;

    // Start is called before the first frame update
    void Start()
    {
        mCanvas = transform.Find("Canvas").GetComponent<Canvas>();
        mCanvas.overrideSorting = true;
        mOrderbar.GetComponent<OrderBar>().Initialize();

        foreach (Transform bar in mCanvas.transform.Find("HealthBarInBattleGroundGroup"))
        {
            if (bar.name == "Borader")
                mHealthBarList.Add(bar.transform.Find("HealthBarInBattleGround").GetComponent<BigHealthBar>());
        }

        mBossHealthBar = mCanvas.transform.Find("BossBorader").Find("HealthBarInBattleGround").GetComponent<BossHealthBar>();

        GameManager.Instance.onFadeGameOverScreenEvent += FadeInScreen;
        GameManager.Instance.onFadeGameOverScreenEvent += FadeInWord;

        GameManager.Instance.onGameOverToReset += FadeOutScreen;
        GameManager.Instance.onGameOverToReset += FadeOutWord;
        GameManager.Instance.onGameOverToReset += ResetUI;

        BattleManager.Instance.onEnqueuingOrderEvent += BattleStart;
        BattleManager.Instance.onFinishOrderEvent += BattleEnd;
        mFadeScreen.SetActive(false);
        mInventoryUI = mCanvas.transform.Find("Inventory").GetComponent<InventoryUI>();
        mDialogueText = mDialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        mYesButton = mDialogueBox.transform.Find("YesButton").GetComponent<Button>();
        mNoButton = mDialogueBox.transform.Find("NoButton").GetComponent<Button>();
        mExitButton = mDialogueBox.transform.Find("ExitButton").GetComponent<Button>();
        mEKeyButton = mDialogueBox.transform.Find("E_key").GetComponent<Image>();
        mVictoryScreen = mCanvas.transform.Find("VictoryScreen").GetComponent<VictoryScreen>();
        mMerchantScreen = mCanvas.transform.Find("MerchantBox").GetComponent<MerchantScreen>();
        mOptionScreenUI = mCanvas.transform.Find("OptionScreen").GetComponent<OptionScreen>();

        mScreenTransition = mCanvas.transform.Find("ScreenTransition").gameObject;
        mTransitionAnimator = mScreenTransition.GetComponent<Animator>();                
        mVictoryScreenTransition = mCanvas.transform.Find("ScreenTransitionVictory").gameObject;
        mVictoryTransitionAnimator = mVictoryScreenTransition.GetComponent<Animator>();
        mScreenTransition.SetActive(false);
        mVictoryScreenTransition.SetActive(false);

        mInventoryUI.Initialize();
        mVictoryScreen.Initialize();
        mYesButton.onClick.RemoveAllListeners();
        mNoButton.onClick.RemoveAllListeners();
        DisplayBattleInterface(false);
        DisplayAskingSkill(false);
        DisplayText(false);
        DisplayDialogueBox(false);
        DisplayInventory(false);
    }

    public void DisplayInventory(bool active)
    {
        mInventoryUI.gameObject.SetActive(false);
    }

    bool switchOfInventory = false;
    float mTime = 0.0f;
    float mCoolTime = 1.0f;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I) && mTime <= 0.0f)
        {
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mOpenInventorySFX);
            mInventoryUI.Active(!switchOfInventory);
            switchOfInventory = !switchOfInventory;
            mTime += mCoolTime;
        }
        if (mTime >= 0.0f)
            mTime -= Time.deltaTime;
    }
  
    public IEnumerator VictoryTransition()
    {
        mVictoryScreenTransition.SetActive(true);
        mVictoryTransitionAnimator.Play("Expand");
        yield return new WaitForSeconds(3.0f);
        mVictoryScreenTransition.SetActive(false);
    }

    public static void ResetUI()
    {
        Instance.mYesButton.onClick.RemoveAllListeners();
        Instance.mNoButton.onClick.RemoveAllListeners();
        Instance.DisplayBattleInterface(false);
        Instance.DisplayAskingSkill(false);
        Instance.DisplayDialogueBox(false);
    }

    public void BattleStart()
    {
        FadeInScreen(() => { StopFade(); });
        mScreenTransition.SetActive(true);
        mTransitionAnimator.Play("Expand");
    }

    public void BattleEnd()
    {
        for (int i = 0; i < mHealthBarList.Count; i++)
        {
            mHealthBarList[i].Active(false);
        }
        mOrderbar.GetComponent<OrderBar>().Clear();
        mOrderbar.gameObject.SetActive(false);
        mScreenTransition.SetActive(false);
    }

    public void DisplayBattleInterface(bool display)
    {
        mBattleUI.SetActive(display);
    }

    public void DisplayAskingSkill(bool display)
    {
        mSkillUseCheck.SetActive(display);
    }
    public void DisplayText(bool display)
    {
        mBasicText.SetActive(display);
    }

    public void DisplayOptionScreen(bool display)
    {
        mOptionScreenUI.Active(display);
    }

    public void DisplayDialogueBox(bool display)
    {
        if (display)
            mDialogueBox.SetActive(display);
        else
            StartCoroutine(EndOfDialogueBox());
    }

    private IEnumerator EndOfDialogueBox()
    {
        mDialogueBox.GetComponent<Animator>().SetTrigger("Outro");
        yield return new WaitForSeconds(1.0f);
        mDialogueBox.SetActive(false);
        mDialogueBox.GetComponent<Animator>().ResetTrigger("Outro");
    }

    public void DisplayHealthBar(bool display)
    {
        for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
        {
            if(display)
            {
                var unit = PlayerController.Instance.mHeroes[i].GetComponent<Player>();
                mHealthBarList[i].Initialize(unit.mSetting.Name,
                    unit.mStatus.mHealth,
                    unit.mStatus.mMaxHealth,
                    unit.mStatus.mMana,
                    unit.mStatus.mMaxMana); 
                unit.mMyHealthBar = mHealthBarList[i];
            }
            mHealthBarList[i].gameObject.SetActive(display);
            mHealthBarList[i].Active(display);
        }
        for (int i = 0; i < BattleManager.Instance.mUnits.Count; ++i)
        {
            if (BattleManager.Instance.mUnits[i].GetComponent<Unit>().GetType() == typeof(Boss))
            {
                if(display)
                {
                    mBossHealthBar.Initialize(
    BattleManager.Instance.mUnits[i].GetComponent<Unit>().mStatus.mHealth,
    BattleManager.Instance.mUnits[i].GetComponent<Unit>().mStatus.mMaxHealth);
                    BattleManager.Instance.mUnits[i].GetComponent<Boss>().mMyHealthBar = mBossHealthBar;
                }

                mBossHealthBar.gameObject.SetActive(display);
                mBossHealthBar.Active(display);

                break;
            }
        }
    }

    public void ChangeText_Skill(string text)
    {
        mSkillUseCheck.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeHoverTip(string text, string action)
    {
        if (action == "Skill")
            mSkillDescription.GetComponent<HoverTip>().mTipToShow = text;
        else if (action == "Attack")
            mAttackDescription.GetComponent<HoverTip>().mTipToShow = text;
        else if (action == "Defend")
            mDefendDescription.GetComponent<HoverTip>().mTipToShow = text;
        else
            Debug.LogWarning("<color=yellow>Warning! " + "</color>" + action + " doesn't exist!");
    }

    public void ChangeText(string text)
    {
        mBasicText.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeDialogueText(string text)
    {
        mDialogueText.GetComponent<TextMeshProUGUI>().text = text;
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

        UIManager.Instance.DisplayHealthBar(true);
        mOrderbar.gameObject.SetActive(true);
    }

    public void FadeInScreen(Action action = null)
    {
        mFadeScreen.SetActive(true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn",true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", false);
        action?.Invoke();
    }

    public void FadeInWord(Action action = null)
    {
        DisplayText(true);
        mBasicText.GetComponent<Animator>().SetBool("FadeIn", true);
        mBasicText.GetComponent<Animator>().SetBool("FadeOut", false);
        action?.Invoke();
    }

    public void FadeOutScreen()
    {
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn", false);
    }

    public void FadeOutWord()
    {
        mBasicText.GetComponent<Animator>().SetBool("FadeIn", false);
        mBasicText.GetComponent<Animator>().SetBool("FadeOut", true);
    }

    public void AddListenerYesButton(UnityAction action = null)
    {
        mYesButton.onClick.RemoveAllListeners();
        mYesButton.onClick.AddListener(action);
    }

    public void AddListenerNoButton(UnityAction action = null)
    {
        mNoButton.onClick.RemoveAllListeners();
        mNoButton.onClick.AddListener(action);
    }    
    
    public void AddListenerExitButton(UnityAction action = null)
    {
        mExitButton.onClick.RemoveAllListeners();
        mExitButton.onClick.AddListener(action);
    }

    public void DisplayButtonsInDialogue(bool action)
    {
        mEKeyButton.gameObject.SetActive(!action);
        mYesButton.gameObject.SetActive(action);
        mNoButton.gameObject.SetActive(action);
    }

    public void DisplayExitButtonInDialogue(bool action)
    {
        mEKeyButton.gameObject.SetActive(!action);
        mExitButton.gameObject.SetActive(action);
    }
}
