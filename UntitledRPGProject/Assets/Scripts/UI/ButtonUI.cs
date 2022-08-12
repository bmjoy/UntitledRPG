using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public enum AudioLocation
    {
        MainMenu,
        Pause
    }
    [SerializeField]
    private AudioLocation m_Location;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(m_Location)
        {
            case AudioLocation.MainMenu:
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mMainMenuClickSFX);
                break;
                case AudioLocation.Pause:
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mPauseButtonClickSFX);
                break;
            default:
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (m_Location)
        {
            case AudioLocation.MainMenu:
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mMainMenuHoverSFX);
                break;
            case AudioLocation.Pause:
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mPauseButtonHoverSFX);
                break;
            default:
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (m_Location)
        {
            case AudioLocation.MainMenu:
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mMainMenuExitSFX);
                break;
            case AudioLocation.Pause:
                AudioManager.PlaySfx(AudioManager.Instance.mAudioStorage.mPauseButtonExitSFX);
                break;
            default:
                break;
        }
    }
}
