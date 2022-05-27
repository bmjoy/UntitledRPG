using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGActionTrigger : ActionTrigger
{

    protected override IEnumerator Action()
    {
        mTime = (GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) / 2.0f;
        GameObject slash = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Temple_Guardian_Slash"), GetComponent<Boss>().mTarget.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(mTime);
        if (DoubleAttackPattern())
        {
            slash.GetComponent<Animator>().SetTrigger("Second");
            GetComponent<Boss>().mAnimator.SetBool("Attack2", true);
            if (GetComponent<Boss>().mBuffNerfController.GetBuffCount() > 0)
                StartCoroutine(CameraSwitcher.Instance.ShakeCamera(GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
            if (GetComponent<Boss>().mTarget)
            {
                GetComponent<Boss>().mTarget.TakeDamage(GetComponent<Boss>().mStatus.mDamage + GetComponent<Boss>().mBonusStatus.mDamage, DamageType.Physical);
                if (GetComponent<Boss>().mTarget.mBuffNerfController.SearchBuff("Counter"))
                {
                    yield return new WaitForSeconds(0.5f);
                    GetComponent<Boss>().mTarget.mTarget = GetComponent<Boss>();
                    GetComponent<Boss>().mTarget.mTarget.TakeDamage(GetComponent<Boss>().mTarget.mStatus.mDamage, DamageType.Magical);
                }
            }

            mTime += (GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime / 3.0f) -0.2f;
            yield return new WaitForSeconds(mTime);
            if (GetComponent<Boss>().mBuffNerfController.GetBuffCount() > 0)
                StartCoroutine(CameraSwitcher.Instance.ShakeCamera(GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
            if (GetComponent<Boss>().mTarget)
            {
                GetComponent<Boss>().mTarget.TakeDamage(GetComponent<Boss>().mStatus.mDamage + GetComponent<Boss>().mBonusStatus.mDamage, DamageType.Physical);
                if (GetComponent<Boss>().mTarget.mBuffNerfController.SearchBuff("Counter"))
                {
                    yield return new WaitForSeconds(0.5f);
                    GetComponent<Boss>().mTarget.mTarget = GetComponent<Boss>();
                    GetComponent<Boss>().mTarget.mTarget.TakeDamage(GetComponent<Boss>().mTarget.mStatus.mDamage, DamageType.Magical);
                }
            }
        }
        else
        {
            if (GetComponent<Boss>().mBuffNerfController.GetBuffCount() > 0)
                StartCoroutine(CameraSwitcher.Instance.ShakeCamera(GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
            if (GetComponent<Boss>().mTarget)
            {
                GetComponent<Boss>().mTarget.TakeDamage(GetComponent<Boss>().mStatus.mDamage + GetComponent<Boss>().mBonusStatus.mDamage, DamageType.Physical);
                if (GetComponent<Boss>().mTarget.mBuffNerfController.SearchBuff("Counter"))
                {
                    yield return new WaitForSeconds(0.5f);
                    GetComponent<Boss>().mTarget.mTarget = GetComponent<Boss>();
                    GetComponent<Boss>().mTarget.mTarget.TakeDamage(GetComponent<Boss>().mTarget.mStatus.mDamage, DamageType.Magical);
                }
            }
        }
        yield return new WaitForSeconds(mTime);
        Destroy(slash);
        GetComponent<Boss>().mAnimator.SetBool("Attack2", false);
        if (GetComponent<Boss>().mStatus.mHealth > 0.0f)
            GetComponent<Boss>().mAiBuild.actionEvent = ActionEvent.BackWalk;
        yield return new WaitUntil(() => GetComponent<Boss>().mAiBuild.actionEvent == ActionEvent.Busy);
        GetComponent<Boss>().TurnEnded();
    }

    protected override void StartActionTrigger()
    {
        mPos = GetComponent<Unit>().mTarget.transform.position;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        StartCoroutine(Action());
    }

    void Start()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;
    }
    private bool DoubleAttackPattern()
    {
        var unit = GetComponent<Boss>();
        return UnityEngine.Random.Range(0, 100) >= unit.mAttackTriggerPercentage;
    }
}