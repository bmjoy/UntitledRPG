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
    public AudioClip mDefendSFX;
    public AudioClip mDodgeSFX;
    public AudioClip mBreakSFX;

    public void ChangeMusic(string name)
    {

        AudioManager.Instance.musicSource.Stop();

        switch(name)
        {
            case "MainMenu":
                AudioManager.Instance.musicSource.clip = (mMainMenuMusic) ? mMainMenuMusic : null;
                AudioManager.FadeInMusic();
                break;
            case "Background":
                AudioManager.Instance.musicSource.clip = (mBackGroundMusic) ? mBackGroundMusic : null;
                AudioManager.FadeInMusic();
                break;
            case "Battle":
                AudioManager.Instance.musicSource.clip = (mBattleMusic) ? mBattleMusic : null;
                AudioManager.FadeInMusic();
                break;
            case "Boss":
                AudioManager.Instance.musicSource.clip = (mBossMusic) ? mBossMusic : null;
                AudioManager.FadeInMusic();
                break;
            case "Victory":
                AudioManager.Instance.musicSource.clip = (mVictoryMusic) ? mVictoryMusic : null;
                AudioManager.Instance.musicSource.Play();
                break;
            case "Defeat":
                AudioManager.Instance.musicSource.clip = (mDefeatMusic) ? mDefeatMusic : null;
                AudioManager.Instance.musicSource.Play();
                break;
        }
    }
}
