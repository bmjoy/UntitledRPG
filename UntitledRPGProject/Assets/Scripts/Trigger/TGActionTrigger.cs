using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Temple Guardian's action trigger
// Content: The trigger includes Temple Guardian's double attacks

public class TGActionTrigger : ActionTrigger
{
    [SerializeField]
    private float mAttackTriggerPercentage = 50.0f;
    [SerializeField]
    private float mShakeTime = 1.0f;
    private bool mTriggered = false;
    protected override IEnumerator Action()
    {
        var boss = GetComponent<Boss>();
        GameObject slash = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Temple_Guardian_Slash"), mPos, Quaternion.identity);

        if (mTriggered)
        {
            yield return new WaitForSeconds(mTime / 3.0f);
            DamageState();
            yield return new WaitForSeconds(mTime / 2.0f);
            slash.GetComponent<Animator>().SetTrigger("Second");
            DamageState();
        }
        else
        {
            yield return new WaitForSeconds(mTime / 2.0f);
            DamageState(); 
        }
        yield return new WaitForSeconds(1.0f);
        Destroy(slash);
        GetComponent<Boss>().mAnimator.SetBool("Attack2", false);
    }

    void DamageState()
    {
        var boss = GetComponent<Boss>();
        if (boss.mBuffNerfController.GetBuffCount() > 0)
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mShakeTime));
        if (boss.mTarget)
        {
            if(GetComponent<Unit>().mAttackClips.Count > 0)
                AudioManager.PlaySfx(GetComponent<Unit>().mAttackClips[Random.Range(0, GetComponent<Unit>().mAttackClips.Count - 1)].Clip,0.6f);
            boss.mTarget.TakeDamage(boss.mStatus.mDamage + boss.mBonusStatus.mDamage, DamageType.Physical);
            StartCoroutine(boss.CounterState(boss.mTarget.mStatus.mDamage));
        }
    }

    protected override void StartActionTrigger()
    {
        var boss = GetComponent<Boss>();
        if (UnityEngine.Random.Range(0, 100) >= mAttackTriggerPercentage)
        {
            mTriggered = true;
            GetComponent<Boss>().mAnimator.SetBool("Attack2", true);
            mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.2f);
        }
        else
        {
            mTriggered = false;
            mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.2f) / 2.0f;
        }
        mPos = GetComponent<Unit>().mTarget.transform.position;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        StartCoroutine(Action());
    }

    void Start()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;
    }

    private void OnDestroy()
    {
        GetComponent<Unit>().mActionTrigger -= StartActionTrigger;
    }
}
