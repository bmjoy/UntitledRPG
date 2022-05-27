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

    public float mAttackTriggerPercentage = 50.0f;
    public float[] mHealthTriggerPercentage;

    protected override void Update()
    {
        base.Update();
    }

    public override IEnumerator AttackAction(DamageType type, Action onComplete)
    {
        if(mTarget)
        {
            mSpriteRenderer.sortingOrder = (transform.position.z < mTarget?.transform.position.z) ? 3 : 4;
            mTarget.mSpriteRenderer.sortingOrder = (transform.position.z > mTarget?.transform.position.z) ? 3 : 4;

            onComplete?.Invoke();
            mAiBuild.actionEvent = ActionEvent.AttackWalk;
            yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
            PlayAnimation("Attack");
            
            mActionTrigger?.Invoke();

        }
    }

    public override void TakeDamage(float dmg, DamageType type)
    {
        base.TakeDamage(dmg, type);
        mMyHealthBar.mCurrentHealth = (mStatus.mHealth > 0.0f) ? mStatus.mHealth : 0.0f;
        if(mMyHealthBar.mCurrentHealth > 0.0f)
            mMyHealthBar.StartCoroutine(mMyHealthBar.PlayBleed());
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
