using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    public GameObject mOrderbar;

    // Start is called before the first frame update
    void Start()
    {
        mCanvas = transform.Find("Canvas").GetComponent<Canvas>();
        mOrderbar.GetComponent<OrderBar>().Initialize();
        BattleManager.Instance.onEnqueuingOrderEvent += BattleStart;
        BattleManager.Instance.onFinishOrderEvent += BattleEnd;
        mFadeScreen.SetActive(false);
        DisplayBattleInterface(false);
        DisplayAskingSkill(false);
        DisplayText(false);
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
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn", false);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", true);
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

}
