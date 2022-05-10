using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public UnitDataStorage mStorage;

    protected override void Start()
    {
        base.Start();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Player");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
            }
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
    }

    public override void TakeDamage(float dmg, DamageType type)
    {
        base.TakeDamage(dmg, type);
        if (mConditions.isDied)
        {
            Debug.Log("Delete");
            GameManager.Instance.mUnitData.Remove(mSetting.Name);
            mStorage = null;
        }
    }

    private void OnDisable()
    {
        if(mStorage != null)
            GameManager.Instance.mUnitData[mSetting.Name] = mStorage;
        mStorage?.SaveData(ref mStatus);

    }
}
