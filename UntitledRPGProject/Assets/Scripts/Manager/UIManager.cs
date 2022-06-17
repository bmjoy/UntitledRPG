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
    
    public UIStorage mStorage;
    public Canvas mCanvas;

    // ----- Animator -----
    private Animator mVictoryTransitionAnimator;
    private Animator mTransitionAnimator;
    private Animator mTextAnimator;
    private Animator mFadeAnimator;

    private List<BigHealthBar> mHealthBarList = new List<BigHealthBar>();
    private BossHealthBar mBossHealthBar;

    public OrderBar mOrderBar;

    public InventoryUI mInventoryUI;
    // ----- Screen -----
    public OptionScreen mOptionScreenUI;
    public VictoryScreen mVictoryScreen;
    public MerchantScreen mMerchantScreen;

    private bool switchOfInventory = false;
    private float mTime = 0.0f;
    private float mCoolTime = 1.0f;

    void Start()
    {
        mCanvas = transform.Find("Canvas").GetComponent<Canvas>();
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
        mTransitionAnimator = mStorage.mScreenTransition.GetComponent<Animator>();
        mTextAnimator = mStorage.mBasicText.GetComponent<Animator>();
        mFadeAnimator = mStorage.mFadeScreen.GetComponent<Animator>();
        mVictoryTransitionAnimator = mStorage.mVictoryScreenTransition.GetComponent<Animator>();
        mOrderBar = mStorage.mOrderbar.GetComponent<OrderBar>();
        mInventoryUI.Initialize();
        mVictoryScreen.Initialize();
        DisplayExitButtonInDialogue(false);
        DisplayBackButtonInDialogue(false);
        DisplayBattleInterface(false);
        DisplayText(false);
        DisplayDialogueBox(false);
        DisplayInventory(false);
    }

    public void DisplayInventory(bool active)
    {
        mInventoryUI.Active(active);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I) && mTime <= 0.0f && GameManager.Instance.mGameState == GameState.GamePlay)
        {
            AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mOpenInventorySFX);
            DisplayInventory(!switchOfInventory);
            switchOfInventory = !switchOfInventory;
            mTime += mCoolTime;
        }
        if (mTime >= 0.0f)
            mTime -= Time.deltaTime;
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
        Instance.mStorage.mLeftButton.onClick.RemoveAllListeners();
        Instance.mStorage.mRightButton.onClick.RemoveAllListeners();
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
        if (display)
        {
            mStorage.mDialogueText.gameObject.SetActive(display);
            mStorage.EKeyImage.gameObject.SetActive(display);
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
        mStorage.EKeyImage.gameObject.SetActive(false);
        mStorage.mLeftButton.gameObject.SetActive(false);
        mStorage.mRightButton.gameObject.SetActive(false);
        mStorage.mDialogueBox.SetActive(false);
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

        UIManager.Instance.DisplayHealthBar(true);
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

    public void AddListenerLeftButton(UnityAction action = null)
    {
        mStorage.mLeftButton.onClick.RemoveAllListeners();
        mStorage.mLeftButton.onClick.AddListener(action);
    }

    public void AddListenerRightButton(UnityAction action = null)
    {
        mStorage.mRightButton.onClick.RemoveAllListeners();
        mStorage.mRightButton.onClick.AddListener(action);
    }    
    
    public void AddListenerExitButton(UnityAction action = null)
    {
        mStorage.mExitButton.onClick.RemoveAllListeners();
        mStorage.mExitButton.onClick.AddListener(action);
    }
    
    public void AddListenerBackButton(UnityAction action = null)
    {
        mStorage.mBackButton.onClick.RemoveAllListeners();
        mStorage.mBackButton.onClick.AddListener(action);
    }

    public void DisplayButtonsInDialogue(bool action)
    {
        mStorage.mLeftButton.gameObject.SetActive(action);
        mStorage.mRightButton.gameObject.SetActive(action);
    }

    public void DisplayExitButtonInDialogue(bool action)
    {
        mStorage.mExitButton.gameObject.SetActive(action);
    }
    
    public void DisplayEKeyInDialogue(bool action)
    {
        mStorage.EKeyImage.gameObject.SetActive(action);
    }
    
    public void DisplayBackButtonInDialogue(bool action)
    {
        mStorage.mBackButton.gameObject.SetActive(action);
    }

    public void ChangeTwoButtons(Sprite left, Sprite right)
    {
        mStorage.mLeftButton.image.sprite = left;
        mStorage.mRightButton.image.sprite = right;
    }
}
