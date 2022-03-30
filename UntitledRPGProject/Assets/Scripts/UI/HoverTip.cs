using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string mTipToShow = string.Empty;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        HoverTipManager.onMouseOutside();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        HoverTipManager.onMouseOutside();
    }

    private void ShowMessage()
    {
        HoverTipManager.onMouseHover(mTipToShow, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForEndOfFrame();
        ShowMessage();
    }    
}
