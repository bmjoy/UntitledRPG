using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverTipManager : MonoBehaviour
{
    public TextMeshProUGUI mTipText;
    public RectTransform mTipWindow;

    public static Action<string, Vector2> onMouseHover;
    public static Action onMouseOutside;

    private void OnEnable()
    {
        onMouseHover += ShowTip;
        onMouseOutside += HideTip;
    }

    private void OnDisable()
    {
        onMouseHover -= ShowTip;
        onMouseOutside -= HideTip;
    }

    // Start is called before the first frame update
    void Start()
    {
        HideTip();
    }

    private void ShowTip(string tip, Vector2 pos)
    {
        mTipText.text = tip;
        mTipWindow.sizeDelta = new Vector2(mTipText.preferredWidth, mTipText.preferredHeight);
        mTipWindow.gameObject.SetActive(true);
        mTipWindow.transform.position = new Vector2(pos.x + mTipWindow.sizeDelta.x * 1.5f, pos.y + 50);
    }

    private void HideTip()
    {
        mTipText.text = default;
        mTipWindow.gameObject.SetActive(false);
    }
}
