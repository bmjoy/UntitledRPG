using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverTipManager : MonoBehaviour
{
    public TextMeshProUGUI mTipText;
    public RectTransform mTipWindow;

    public static Action<string, Vector3> onMouseHover;
    public static Action onMouseOutside;
    [SerializeField]
    private int mWidth = 35;
    [SerializeField]
    private int mHeight = 50;

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

    private void ShowTip(string tip, Vector3 pos)
    {
        mTipText.text = tip;
        mTipWindow.sizeDelta = new Vector2(mTipText.preferredWidth, mTipText.preferredHeight);
        mTipWindow.gameObject.SetActive(true);
        mTipWindow.transform.localPosition = new Vector3((pos.x / mWidth) - mWidth, (pos.y / mHeight) - mHeight, 0.0f);
    }

    private void HideTip()
    {
        mTipText.text = default;
        mTipWindow.gameObject.SetActive(false);
    }
}
