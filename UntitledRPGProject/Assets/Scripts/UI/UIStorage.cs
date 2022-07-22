using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class UIStorage : MonoBehaviour
{
    [TextArea]
    public string mTextForTarget;
    [TextArea]
    public string mTextForAccpet;
    [TextArea]
    public string mTextForGameOver = "Wasted!";

    public GameObject mSkillDescription;
    public GameObject mAttackDescription;
    public GameObject mDefendDescription;

    public GameObject mBattleUI;
    public GameObject mBasicText;
    public GameObject mDialogueBox;
    public GameObject mRequireMoneyBox;
    public GameObject mFadeScreen;
    public GameObject mOrderbar;
    public GameObject mScreenTransition;
    public GameObject mVictoryScreenTransition;
    public GameObject mMinimap;

    public GameObject mTutorialWASDIcon;
    public GameObject mTutorialInteractIcon;
    public GameObject mTutorialItemIcon;
    public GameObject mTutorialHereIcon;

    public GameObject mCurrentMoney;
    public GameObject mSupportKey;
    public TextMeshProUGUI mDialogueText;
    public void Initialize()
    {
        mRequireMoneyBox = mDialogueBox.transform.Find("CompanionBox").gameObject;
        mDialogueText = mDialogueBox.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        mMinimap = UIManager.Instance.mCanvas.transform.Find("MinimapWindow").gameObject;
        mOrderbar.GetComponent<OrderBar>().Initialize();
        mScreenTransition = UIManager.Instance.mCanvas.transform.Find("ScreenTransition").gameObject;
        mCurrentMoney = UIManager.Instance.mCanvas.transform.Find("MyMoney").gameObject;
        mVictoryScreenTransition = UIManager.Instance.mCanvas.transform.Find("ScreenTransitionVictory").gameObject;
        mFadeScreen.SetActive(false);
        mScreenTransition.SetActive(false);
        mVictoryScreenTransition.SetActive(false);
        mCurrentMoney.SetActive(false);
        mRequireMoneyBox.SetActive(false);
        mSupportKey.SetActive(false);
    }

    private void Update()
    {
        if(PlayerController.Instance && mCurrentMoney.activeSelf)
        {
            mCurrentMoney.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = PlayerController.Instance.mGold.ToString();
        }
    }
}
