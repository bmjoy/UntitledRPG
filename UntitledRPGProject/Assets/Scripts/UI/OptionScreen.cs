using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScreen : MonoBehaviour
{
    public Slider mMaster_S;
    public Slider mMusic_S;
    public Slider mSFX_S;

    public Button mContinue;
    public Button mBackToMainMenu;
    public Button mExit;
    void Start()
    {
        gameObject.SetActive(false);
        Initialize();
    }

    public void Active(bool active)
    {
        if(active)
            Initialize();
        gameObject.SetActive(active);
        if(GameManager.mGameState == GameState.MainMenu)
        {
            mContinue.gameObject.SetActive(false);
            mBackToMainMenu.gameObject.SetActive(false);
            mExit.gameObject.SetActive(false);
        }
        else
        {
            mContinue.gameObject.SetActive(active);
            mBackToMainMenu.gameObject.SetActive(active);
            mExit.gameObject.SetActive(active);
        }
    }

    void Initialize()
    {
        float val = 0.0f;
        AudioManager.Instance.audioMixer.GetFloat("Master_Volume", out val);
        mMaster_S.value = val;
        AudioManager.Instance.audioMixer.GetFloat("Music_Volume", out val);
        mMusic_S.value = val;
        AudioManager.Instance.audioMixer.GetFloat("SFX_Volume", out val);
        mSFX_S.value = val;
    }
}
