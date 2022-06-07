using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Player : Unit
{
    [HideInInspector]
    public UnitDataStorage mStorage;
    [HideInInspector]
    public BigHealthBar mMyHealthBar;

    public WeaponType mWeaponType;

    protected override void Start()
    {
        base.Start();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Player");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
        }
        mAiBuild.type = AIType.Manual;
        mTarget = null;
    }

    public void Initialize()
    {
        if (!GameManager.Instance.mUnitData.ContainsKey(mSetting.Name))
        {
            GameManager.Instance.mUnitData.Add(mSetting.Name, new UnitDataStorage());
            mStorage = GameManager.Instance.mUnitData[mSetting.Name];
            mStorage.SaveData(ref mStatus);
        }
        else
        {
            mStorage = GameManager.Instance.mUnitData[mSetting.Name];
            mStorage.LoadData(ref mStatus);
        }
    }

    protected override void Update()
    {
        base.Update();
        if(mMyHealthBar != null)
            mMyHealthBar.mCurrentMana = mStatus.mMana;
    }

    public override void TakeDamage(float dmg, DamageType type)
    {
        base.TakeDamage(dmg, type);
        mMyHealthBar.mCurrentHealth = (mStatus.mHealth > 0.0f) ? mStatus.mHealth : 0.0f;
        mMyHealthBar.StartCoroutine(mMyHealthBar.PlayBleed());
        if (mConditions.isDied)
        {
            GameManager.Instance.mUnitData.Remove(mSetting.Name);
            mStorage = null;
        }
    }

    public override void TakeRecover(float val)
    {
        base.TakeRecover(val);
        if(mMyHealthBar)
            mMyHealthBar.mNextHealth = mStatus.mHealth;
    }

    public override void TakeRecoverMana(float val)
    {
        base.TakeRecoverMana(val);
        if (mMyHealthBar)
            mMyHealthBar.mCurrentMana = mStatus.mMana;
    }

    private void OnDisable()
    {
        if(mStorage != null)
            GameManager.Instance.mUnitData[mSetting.Name] = mStorage;
        mStorage?.SaveData(ref mStatus);
    }
}
