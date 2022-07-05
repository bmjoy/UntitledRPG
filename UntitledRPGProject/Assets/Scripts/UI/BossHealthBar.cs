using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBar
{
    private Image mDamagedHealthBar;
    public float mNextHealth = 0.0f;
    private TextMeshProUGUI mHPText;
    private void Start()
    {
        mBorader = transform.GetComponent<Image>();
        mAnimator = transform.parent.GetComponent<Animator>();
        mBar = transform.Find("Health").GetComponent<Image>();
        mDamagedHealthBar = transform.Find("DamagedHealth").GetComponent<Image>();
        mHPText = transform.Find("HPValue").GetComponent<TextMeshProUGUI>();
        Active(false);
    }

    public override void Initialize(float currHP, float maxHP, float currMP, float maxMP)
    {
        mCurrentHealth = currHP;
        mMaxHealth = maxHP;
        mHPText.text = mCurrentHealth.ToString();
        mBar.fillAmount = mCurrentHealth / mMaxHealth;
        isInitialized = true;
        Active(true);
    }

    protected override void Update()
    {
        if (isInitialized)
        {
            mBar.fillAmount = (mNextHealth > mCurrentHealth) ? Mathf.Lerp(mBar.fillAmount, mNextHealth / mMaxHealth, Time.deltaTime * 1.5f)
                : mCurrentHealth / mMaxHealth;
            if ((mNextHealth > mCurrentHealth) && mBar.fillAmount >= (mNextHealth / mMaxHealth) - 0.01f)
            {
                mCurrentHealth = mNextHealth;
                mNextHealth = 0.0f;
            }
            mDamagedHealthBar.fillAmount = Mathf.Lerp(mDamagedHealthBar.fillAmount, mCurrentHealth / mMaxHealth, Time.deltaTime * 1.5f);
            mHPText.text = mCurrentHealth.ToString();
        }
    }

    public override void Active(bool active)
    {
        base.Active(active);
        mDamagedHealthBar?.gameObject.SetActive(active);
        mHPText?.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    public override IEnumerator PlayBleed()
    {
        mAnimator.SetTrigger("Wiggle");
        yield return new WaitForSeconds(0.51f);
        if (mCurrentHealth <= 0.0f)
            Active(false);
    }
}
