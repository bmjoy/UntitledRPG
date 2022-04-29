using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image mBorader;
    public Image mBar;

    private float mMaxHealth = 0.0f;
    public float mCurrentHealth = 0.0f;

    private bool isInitialized = false;
    public void Initialize(float curr, float max)
    {
        mCurrentHealth = curr;
        mMaxHealth = max;
        mBorader = transform.parent.GetComponent<Image>();
        mBar = GetComponent<Image>();
        isInitialized = true;
    }

    void Update()
    {
        if(isInitialized)
        {
            mBorader.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            mBar.fillAmount = mCurrentHealth / mMaxHealth;
        }

    }

    public void Active(bool active)
    {
        mBorader.gameObject.SetActive(active);
        mBar.gameObject.SetActive(active);
    }

    private void OnDestroy()
    {
        Destroy(mBorader?.gameObject);
    }
}
