using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : Enemy
{
    [Serializable]
    public class BossPatterns
    {
        public enum Patterns
        {
            DoubleAttack,
            MagicWhenHalfHealth
        }
        public Patterns mPattern;
        public int mPercentage;
    }

    public List<BossPatterns> mBossPatterns;
    [HideInInspector]
    public bool _doubleAttack = false;
    [HideInInspector]
    public bool _magicWhenHalfHealth = false;
    [HideInInspector]
    public BossHealthBar mMyHealthBar;

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
            
            foreach(BossPatterns patterns in mBossPatterns)
            {
                if(patterns.mPattern == BossPatterns.Patterns.DoubleAttack)
                {
                    _doubleAttack = DoubleAttackPattern(patterns.mPercentage);
                }
            }

            if (_doubleAttack)
            {
                PlayAnimation("Attack1");
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
                if (mBuffNerfController.GetBuffCount() > 0)
                    StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
                if (mTarget)
                {
                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                    if (mTarget.mBuffNerfController.SearchBuff("Counter"))
                    {
                        yield return new WaitForSeconds(0.5f);
                        mTarget.mTarget = this;
                        mTarget.mTarget.TakeDamage(mTarget.mStatus.mDamage, DamageType.Magical);
                    }
                }

                mAnimator.SetBool("Attack2", true);
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
                if (mBuffNerfController.GetBuffCount() > 0)
                   StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
                if (mTarget)
                {
                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                    if (mTarget.mBuffNerfController.SearchBuff("Counter"))
                    {
                        yield return new WaitForSeconds(0.5f);
                        mTarget.mTarget = this;
                        mTarget.mTarget.TakeDamage(mTarget.mStatus.mDamage, DamageType.Magical);
                    }
                }
            }
            else
            {
                PlayAnimation("Attack1");
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
                if(mBuffNerfController.GetBuffCount() > 0)
                    StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
                if (mTarget)
                {
                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
                    if (mTarget.mBuffNerfController.SearchBuff("Counter"))
                    {
                        yield return new WaitForSeconds(0.5f);
                        mTarget.mTarget = this;
                        mTarget.mTarget.TakeDamage(mTarget.mStatus.mDamage, DamageType.Magical);
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
            mAiBuild.actionEvent = ActionEvent.BackWalk;
            yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
            TurnEnded();
        }
    }

    public override void TakeDamage(float dmg, DamageType type)
    {
        base.TakeDamage(dmg, type);
        mMyHealthBar.mCurrentHealth = (mStatus.mHealth > 0.0f) ? mStatus.mHealth : 0.0f;
        mMyHealthBar.StartCoroutine(mMyHealthBar.PlayBleed());
    }

    public override void TakeRecover(float val)
    {
        base.TakeRecover(val);
        mMyHealthBar.mNextHealth = mStatus.mHealth;
    }

    public override void TurnEnded()
    {
        base.TurnEnded();
        mAnimator.SetBool("Attack2", false);
    }

    public bool DoubleAttackPattern(int percentage)
    {
        return UnityEngine.Random.Range(0, 100) >= percentage;
    }
    public bool HalfHealthEvent(float health)
    {
        return Mathf.RoundToInt((100 * mStatus.mHealth) / mStatus.mMaxHealth) <= health;
    }
}
