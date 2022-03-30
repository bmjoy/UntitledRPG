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

    public Canvas mCanvas;
    public GameObject mBattleUI;
    public GameObject mSkillDescription;
    public GameObject mSkillUseCheck;

    // Start is called before the first frame update
    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        DisplayBattleInterface(false);
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
}
