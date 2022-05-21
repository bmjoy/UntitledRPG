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

    [HideInInspector]
    public List<BossPatterns> mBossPatterns;
    [HideInInspector]
    public bool _doubleAttack = false;
    [HideInInspector]
    public bool _magicWhenHalfHealth = false;

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
                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);

                mAnimator.SetBool("Attack2", true);
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
                if (mBuffNerfController.GetBuffCount() > 0)
                   StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
                if (mTarget)
                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
            }
            else
            {
                PlayAnimation("Attack1");
                yield return new WaitForSeconds(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
                if(mBuffNerfController.GetBuffCount() > 0)
                    StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
                if (mTarget)
                    mTarget.TakeDamage(mStatus.mDamage + mBonusStatus.mDamage, type);
            }

            yield return new WaitForSeconds(mWaitingTimeForBattle);
            mAiBuild.actionEvent = ActionEvent.BackWalk;
            yield return new WaitUntil(() => mAiBuild.actionEvent == ActionEvent.Busy);
            TurnEnded();
        }

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
