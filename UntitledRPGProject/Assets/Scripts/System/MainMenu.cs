using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas mCanvas;
    public Button StartGame;
    public Button Tutorial;

    public Slider mMasterSlider;
    public Slider mMusicSlider;
    public Slider mSFXSlider;
    void Start()
    {
        GameManager.Instance.mGameState = GameState.MainMenu;
        AudioManager.Instance.mAudioStorage.ChangeMusic("MainMenu");

        mMasterSlider.value = 0.0f;
        mMusicSlider.value = 0.0f;
        mSFXSlider.value = 0.0f;

        AudioManager.SetMasterVolume(mMasterSlider.value);
        AudioManager.SetMusicVolume(mMusicSlider.value);
        AudioManager.SetSFXVolume(mSFXSlider.value);

        Tutorial.onClick.RemoveAllListeners();
        StartGame.onClick.RemoveAllListeners();

        Tutorial.onClick.AddListener(() => SceneLoader.Instance._sceneIndex = 2);
        Tutorial.onClick.AddListener(() => AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mMainMenuButtonSFX));
        Tutorial.onClick.AddListener(() => SceneLoader.Instance.StartGame());
        Tutorial.onClick.AddListener(() => mCanvas.sortingOrder = -1);
        Tutorial.onClick.AddListener(() => GameManager.Instance.mGameState = GameState.Initialize);

        StartGame.onClick.AddListener(() => SceneLoader.Instance._sceneIndex = 3);
        StartGame.onClick.AddListener(() => AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mMainMenuButtonSFX));
        StartGame.onClick.AddListener(() => SceneLoader.Instance.StartGame());
        StartGame.onClick.AddListener(() => mCanvas.sortingOrder = -1);
        StartGame.onClick.AddListener(() => GameManager.Instance.mGameState = GameState.Initialize);
    }
}
