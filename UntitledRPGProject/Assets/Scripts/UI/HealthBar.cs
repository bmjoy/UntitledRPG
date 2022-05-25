using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    protected Image mBorader;
    protected Image mBar;
    protected Image mManaBar;

    protected Animator mAnimator;

    protected float mMaxHealth = 0.0f;
    public float mCurrentHealth = 0.0f;

    protected float mMaxMana = 0.0f;
    public float mCurrentMana = 0.0f;

    protected bool isInitialized = false;

    public virtual void Initialize(float currHP, float maxHP, float currMP, float maxMP)
    {
        mCurrentHealth = currHP;
        mMaxHealth = maxHP;
        mCurrentMana = currMP;
        mMaxMana = maxMP;

        mBorader = transform.parent.GetComponent<Image>();
        mAnimator = mBorader.GetComponent<Animator>();
        mBorader.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        mBar = GetComponent<Image>();
        mManaBar = transform.Find("ManaBorader").Find("Mana").GetComponent<Image>();
        isInitialized = true;
        gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if(isInitialized)
        {
            mBorader.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
            mBar.fillAmount = mCurrentHealth / mMaxHealth;
            mManaBar.fillAmount = mCurrentMana / mMaxMana;
        }
    }

    public virtual IEnumerator PlayBleed()
    {
        mBorader.transform.GetComponent<Animator>().SetTrigger("Wiggle");
        transform.Find("Bleed").GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.51f);
        transform.Find("Bleed").transform.localPosition = new Vector3(mBar.fillAmount * 100.0f - 50.0f, 0.0f, 0.0f);
    }

    public virtual void Active(bool active)
    {
        mBorader.gameObject?.SetActive(active);
        mBar.gameObject?.SetActive(active);
        mManaBar?.gameObject?.SetActive(active);
    }

    private void OnDestroy()
    {
        Destroy(mBorader?.gameObject);
    }
}
