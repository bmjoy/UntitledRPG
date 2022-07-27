using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private static AudioManager mInstance;
    public static AudioManager Instance { get { return mInstance; } }
    public AudioStorage mAudioStorage;

    [SerializeField]
    private float mFadeTime = 2.0f;

    static float Master_vol;
    static float Music_vol;
    static float SFX_vol;

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
        musicSource.loop = true;
        musicSource.volume = 0.3f;
        mAudioStorage = GetComponent<AudioStorage>();
    }

    private void Start()
    {
        Instance.audioMixer.GetFloat("Master_Volume", out Master_vol);
        Instance.audioMixer.GetFloat("Music_Volume", out Music_vol);
        Instance.audioMixer.GetFloat("SFX_Volume", out SFX_vol);
    }

    public static void FadeOutMusic()
    {
        mInstance.StartCoroutine(FadeOut(Instance.mFadeTime));
    }
    public static void FadeInMusic()
    {
        mInstance.StartCoroutine(FadeIn(Instance.mFadeTime));
    }

    public static void PlaySfx(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null)
            return;
        Instance.sfxSource.PlayOneShot(clip,volume);
    }

    private static IEnumerator BeginFadeOut(float duration)
    {
        var muteMusicSS = Instance.audioMixer.FindSnapshot("MuteMusic");
        Instance.audioMixer.TransitionToSnapshots(new AudioMixerSnapshot[] { muteMusicSS },
                                            new float[] { 1.0f },
                                            duration);

        yield return new WaitForSeconds(duration);
        // at this point the transition is done.

        var defaultSS = Instance.audioMixer.FindSnapshot("Default");
        Instance.audioMixer.TransitionToSnapshots(new AudioMixerSnapshot[] { defaultSS },
                                     new float[] { 1.0f },
                                     duration);
    }

    public static IEnumerator FadeOut(float FadeTime)
    {
        float startVolume = Instance.musicSource.volume;

        while (Instance.musicSource.volume > 0)
        {
            Instance.musicSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        Instance.musicSource.Stop();
        Instance.musicSource.volume = startVolume;
    }

    public static IEnumerator FadeIn(float FadeTime)
    {
        float startVolume = 0.2f;
        Instance.musicSource.volume = 0;
        Instance.musicSource.Play();

        while (Instance.musicSource.volume < 1.0f)
        {
            Instance.musicSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
        Instance.musicSource.volume = 1.0f;
    }

    static bool masterMute = false;

    public static void MuteVolume(string vol_Name)
    {
        masterMute = !masterMute;
        if (masterMute)
        {
            switch(vol_Name)
            {
                case "Master":
                    Instance.audioMixer.GetFloat("Master_Volume", out Master_vol);
                    SetMasterVolume(-50.0f);
                    break;
                case "Music":
                    Instance.audioMixer.GetFloat("Music_Volume", out Music_vol);
                    SetMusicVolume(-50.0f);
                    break;
                case "SFX":
                    Instance.audioMixer.GetFloat("SFX_Volume", out SFX_vol);
                    SetSFXVolume(-50.0f);
                    break;
                default:
                    Debug.LogWarning("<color=yellow>Warning!</color> The name of volume is not valid!");
                    break;
            }
        }
        else
        {
            switch (vol_Name)
            {
                case "Master":
                    SetMasterVolume(Master_vol);
                    break;
                case "Music":
                    SetMusicVolume(Music_vol);
                    break;
                case "SFX":
                    SetSFXVolume(SFX_vol);
                    break;
                default:
                    Debug.LogWarning("<color=yellow>Warning!</color> The name of volume is not valid!");
                    break;
            }
        }

    }

    public static void SetMasterVolume(float vol)
    {
        if (vol <= -50.0f)
            vol = -80.0f;
        Instance.audioMixer.SetFloat("Master_Volume", vol);
    }

    public static void SetMusicVolume(float vol)
    {
        if (vol <= -50.0f)
            vol = -80.0f;
        Instance.audioMixer.SetFloat("Music_Volume", vol);
    }

    public static void SetSFXVolume(float vol)
    {
        if (vol <= -50.0f)
            vol = -80.0f;
        Instance.audioMixer.SetFloat("SFX_Volume", vol);
    }
}
