using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BigHealthBar : HealthBar
{
    private TextMeshProUGUI mNameText;
    private TextMeshProUGUI mHPText;
    private TextMeshProUGUI mMPText;

    private Image mDamagedHealthBar;
    private Image mDamagedManaBar;

    private void Start()
    {
        mBorader = transform.GetComponent<Image>();
        mAnimator = transform.parent.GetComponent<Animator>();
        mBar = transform.Find("Health").GetComponent<Image>();
        mManaBar = transform.Find("ManaBorader").Find("Mana").GetComponent<Image>();
        mDamagedHealthBar = transform.Find("DamagedHealth").GetComponent<Image>();
        mDamagedManaBar = transform.Find("ManaBorader").Find("DamagedMana").GetComponent<Image>();
        mNameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        mHPText = transform.Find("HPValue").GetComponent<TextMeshProUGUI>();
        mMPText = transform.Find("MPValue").GetComponent<TextMeshProUGUI>();
        Active(false);
    }

    public void Initialize(string name, float currHP, float maxHP, float currMP, float maxMP)
    {
        mNameText.text = name;
        mCurrentHealth = currHP;
        mMaxHealth = maxHP;
        mCurrentMana = currMP;
        mMaxMana = maxMP;

        mBar.fillAmount = mCurrentHealth / mMaxHealth;
        mManaBar.fillAmount = mCurrentMana / mMaxMana;
        mHPText.text = mCurrentHealth.ToString();
        mMPText.text = mCurrentMana.ToString();

        isInitialized = true;
        Active(true);
    }

    protected override void Update()
    {
        if(isInitialized)
        {
            mBar.fillAmount = mCurrentHealth / mMaxHealth;
            mManaBar.fillAmount = mCurrentMana / mMaxMana;
            mDamagedHealthBar.fillAmount = Mathf.Lerp(mDamagedHealthBar.fillAmount, mCurrentHealth / mMaxHealth,Time.deltaTime * 1.5f);
            mDamagedManaBar.fillAmount = Mathf.Lerp(mDamagedManaBar.fillAmount, mCurrentMana / mMaxMana,Time.deltaTime * 1.5f);
            mHPText.text = mCurrentHealth.ToString();
            mMPText.text = mCurrentMana.ToString();
        }
    }

    public override void Active(bool active)
    {
        base.Active(active);
        mDamagedHealthBar?.gameObject.SetActive(active);
        mNameText?.gameObject.SetActive(active);
        mHPText?.gameObject.SetActive(active);
        mMPText?.gameObject.SetActive(active);
    }

    public override IEnumerator PlayBleed()
    {
        mAnimator.SetTrigger("Wiggle");
        yield return new WaitForSeconds(0.51f);
    }
}
