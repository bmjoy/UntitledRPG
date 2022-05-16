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
    public GameObject mSkillUseCheck;
    public GameObject mBasicText;
    public GameObject mFadeScreen;
    public GameObject mDialogueBox;

    private List<BigHealthBar> mHealthBarList = new List<BigHealthBar>();

    private TextMeshProUGUI mDialogueText;

    private Button mYesButton;
    private Button mNoButton;

    public GameObject mOrderbar;

    // Start is called before the first frame update
    void Start()
    {
        mCanvas = transform.Find("Canvas").GetComponent<Canvas>();
        mCanvas.overrideSorting = true;
        mOrderbar.GetComponent<OrderBar>().Initialize();

        foreach(Transform bar in mCanvas.transform.Find("HealthBarInBattleGroundGroup"))
        {
            if(bar.name == "Borader")
                mHealthBarList.Add(bar.transform.Find("HealthBarInBattleGround").GetComponent<BigHealthBar>());
        }

        GameManager.Instance.onFadeGameOverScreenEvent += FadeInScreen;
        GameManager.Instance.onFadeGameOverScreenEvent += FadeInWord;

        GameManager.Instance.onGameOverToReset += FadeOutScreen;
        GameManager.Instance.onGameOverToReset += FadeOutWord;
        GameManager.Instance.onGameOverToReset += ResetUI;

        BattleManager.Instance.onEnqueuingOrderEvent += BattleStart;
        BattleManager.Instance.onFinishOrderEvent += BattleEnd;
        mFadeScreen.SetActive(false);
        mDialogueText = mDialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        mYesButton = mDialogueBox.transform.Find("YesButton").GetComponent<Button>();
        mNoButton = mDialogueBox.transform.Find("NoButton").GetComponent<Button>();
        mYesButton.onClick.RemoveAllListeners();
        mNoButton.onClick.RemoveAllListeners();
        DisplayBattleInterface(false);
        DisplayAskingSkill(false);
        DisplayText(false);
        DisplayDialogueBox(false);
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
        mOrderbar.gameObject.SetActive(true);
    }

    public void BattleEnd()
    {
        mOrderbar.GetComponent<OrderBar>().Clear();
        mOrderbar.gameObject.SetActive(false);
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

    public void DisplayDialogueBox(bool display)
    {
        mDialogueBox.SetActive(display);
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
            mHealthBarList[i].Active(display);
        }
    }

    public void ChangeText_Skill(string text)
    {
        mSkillUseCheck.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeHoverTip(string text)
    {
        mSkillDescription.GetComponent<HoverTip>().mTipToShow = text;
    }

    public void ChangeText(string text)
    {
        mBasicText.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeDialogueText(string text)
    {
        mDialogueText.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void FadeInScreen()
    {
        mFadeScreen.SetActive(true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn",true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", false);
    }

    public void FadeInWord()
    {
        DisplayText(true);
        mBasicText.GetComponent<Animator>().SetBool("FadeIn", true);
        mBasicText.GetComponent<Animator>().SetBool("FadeOut", false);
    }

    public void FadeOutScreen()
    {
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn", false);
        StartCoroutine(WaitScreen());
    }

    public void FadeOutWord()
    {
        mBasicText.GetComponent<Animator>().SetBool("FadeIn", false);
        mBasicText.GetComponent<Animator>().SetBool("FadeOut", true);
    }

    private IEnumerator WaitScreen()
    {
        yield return new WaitForSeconds(CameraSwitcher.Instance.mCamera.m_DefaultBlend.m_Time);
        mFadeScreen.SetActive(false);
        DisplayText(false);
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

    public void DisplayButtonsInDialogue(bool action)
    {
        mYesButton.gameObject.SetActive(action);
        mNoButton.gameObject.SetActive(action);
    }
}
