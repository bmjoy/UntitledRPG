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

    protected override void Update()
    {
        base.Update();
        if(mMyHealthBar != null)
            mMyHealthBar.mCurrentMana = mStatus.mMana;
    }

    public override bool TakeDamage(float dmg, DamageType type)
    {
        bool isHit = true;
        isHit = base.TakeDamage(dmg, type);
        mMyHealthBar.mCurrentHealth = (mStatus.mHealth > 0.0f) ? mStatus.mHealth : 0.0f;
        if(isHit && mMyHealthBar.mCurrentHealth > 0.0f)
            mMyHealthBar.StartCoroutine(mMyHealthBar.PlayBleed());
        return isHit;
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
}
