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
        Debug.Log("Hi UIManager");
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Canvas mCanvas;
    public GameObject mBattleUI;
    public GameObject mSkillDescription;
    public GameObject mSkillUseCheck;
    public GameObject mFadeScreen;

    public GameObject mOrderbar;

    // Start is called before the first frame update
    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        BattleManager.Instance.onEnqueuingOrderEvent += BattleStart;
        BattleManager.Instance.onFinishOrderEvent += BattleEnd;
        mFadeScreen.SetActive(false);
        DisplayBattleInterface(false);
    }

    public void BattleStart()
    {
        if(mOrderbar.gameObject.activeInHierarchy == false)
            mOrderbar.gameObject.SetActive(true);
    }

    public void BattleEnd()
    {
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

    public void ChangeText_Target(string text)
    {
        mSkillUseCheck.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeText(string text)
    {
        mSkillDescription.GetComponent<HoverTip>().mTipToShow = text;
    }

    public void FadeInScreen()
    {
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn",true);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", false);
    }

    public void FadeInWord()
    {
        mSkillUseCheck.GetComponent<Animator>().SetBool("FadeIn", true);
        mSkillUseCheck.GetComponent<Animator>().SetBool("FadeOut", false);
    }

    public void FadeOutScreen()
    {
        mFadeScreen.GetComponent<Animator>().SetBool("FadeIn", false);
        mFadeScreen.GetComponent<Animator>().SetBool("FadeOut", true);
    }

    public void FadeOutWord()
    {
        mSkillUseCheck.GetComponent<Animator>().SetBool("FadeIn", false);
        mSkillUseCheck.GetComponent<Animator>().SetBool("FadeOut", true);
    }
}
