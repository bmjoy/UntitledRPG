using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : Enemy
{
    [HideInInspector]
    public bool _magicWhenHalfHealth = false;
    [HideInInspector]
    public BossHealthBar mMyHealthBar;
    public float[] mHealthTriggerPercentage;
    [HideInInspector]
    public BossActionTrigger mActionTriggerComponent;

    protected override void Start()
    {
        base.Start();
        mActionTriggerComponent = GetComponent<BossActionTrigger>();
    }

    protected override void Update()
    {
        if(mMyHealthBar != null)
        {
            mMyHealthBar.mCurrentHealth = mStatus.mHealth + mBonusStatus.mHealth;
            mMyHealthBar.mMaxHealth = mStatus.mMaxHealth + mBonusStatus.mHealth;
        }
        base.Update();
    }

    public override bool TakeDamage(float dmg, DamageType type)
    {
        bool isHit = true;
        isHit = base.TakeDamage(dmg, type);
        mMyHealthBar.mCurrentHealth = (mStatus.mHealth > 0.0f) ? mStatus.mHealth : 0.0f;
        if (isHit && mMyHealthBar.mCurrentHealth > 0.0f)
            mMyHealthBar.StartCoroutine(mMyHealthBar.PlayBleed());

        return isHit;
    }

    public override void TakeRecover(float val)
    {
        base.TakeRecover(val);
        mMyHealthBar.mNextHealth = mStatus.mHealth;
    }

    public bool HalfHealthEvent(float health)
    {
        return Mathf.RoundToInt((100 * mStatus.mHealth) / mStatus.mMaxHealth) <= health;
    }
}
