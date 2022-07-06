using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionScreen : MonoBehaviour
{
    public Slider mMaster_S;
    public Slider mMusic_S;
    public Slider mSFX_S;

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
