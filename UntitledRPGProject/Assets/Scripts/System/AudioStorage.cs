using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStorage : MonoBehaviour
{
    public AudioClip mBattleMusic;
    public AudioClip mBossMusic;
    public AudioClip mVictoryMusic;
    public AudioClip mDefeatMusic;
    public AudioClip mBackGroundMusic;
    public AudioClip mMainMenuMusic;

    public AudioClip mItemPurchaseSFX;
    public AudioClip mLevelUPSFX;
    public AudioClip mItemEquipSFX;
    public AudioClip mOpenInventorySFX;
    public AudioClip mExclamationSFX;
    public AudioClip mMainMenuButtonSFX;

    public void ChangeMusic(string name)
    {
        AudioManager.Instance.musicSource.Stop();

        switch(name)
        {
            case "MainMenu":
                AudioManager.Instance.musicSource.clip = (mMainMenuMusic) ? mMainMenuMusic : null;
                break;
            case "Background":
                AudioManager.Instance.musicSource.clip = (mBackGroundMusic) ? mBackGroundMusic : null;
                break;
            case "Battle":
                AudioManager.Instance.musicSource.clip = (mBattleMusic) ? mBattleMusic : null;
                break;
            case "Boss":
                AudioManager.Instance.musicSource.clip = (mBossMusic) ? mBossMusic : null;
                break;
            case "Victory":
                AudioManager.Instance.musicSource.clip = (mVictoryMusic) ? mVictoryMusic : null;
                break;
            case "Defeat":
                AudioManager.Instance.musicSource.clip = (mDefeatMusic) ? mDefeatMusic : null;
                break;
        }
        if(AudioManager.Instance.musicSource.clip)
            AudioManager.Instance.musicSource.Play();
    }
}
