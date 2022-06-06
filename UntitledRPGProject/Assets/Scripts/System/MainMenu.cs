using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    GameObject loader;
    public Canvas mCanvas;
    public Button button;

    public Slider mMasterSlider;
    public Slider mMusicSlider;
    public Slider mSFXSlider;
    void Start()
    {
        loader = GameObject.Find("GameLoader");
        GameManager.Instance.mGameState = GameState.MainMenu;
        AudioManager.Instance.mAudioStorage.ChangeMusic("MainMenu");

        mMasterSlider.value = 0.0f;
        mMusicSlider.value = 0.0f;
        mSFXSlider.value = 0.0f;

        AudioManager.SetMasterVolume(mMasterSlider.value);
        AudioManager.SetMusicVolume(mMusicSlider.value);
        AudioManager.SetSFXVolume(mSFXSlider.value);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mMainMenuButtonSFX));
        button.onClick.AddListener(() => loader.GetComponent<SceneLoader>().StartGame());
        button.onClick.AddListener(() => mCanvas.sortingOrder = -1);
        button.onClick.AddListener(() => GameManager.Instance.mGameState = GameState.Initialize);
    }
}
