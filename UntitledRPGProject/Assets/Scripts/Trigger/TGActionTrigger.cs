using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Temple Guardian's action trigger
// Content: The trigger includes Temple Guardian's double attacks

public class TGActionTrigger : BossActionTrigger
{
    [SerializeField]
    private float mAttackTriggerPercentage = 50.0f;
    [SerializeField]
    private float mShakeTime = 1.0f;
    void Start()
    {
        GetComponent<Boss>().mActionTrigger += StartActionTrigger;
    }
    protected override IEnumerator Action()
    {
        var boss = GetComponent<Boss>();
        GameObject obj = ResourceManager.GetResource<GameObject>("Prefabs/Effects/Temple_Guardian_Slash");
        yield return new WaitForSeconds(mTime / 2.0f);
        GameObject slash = Instantiate(obj, mPos, Quaternion.Euler(obj.transform.eulerAngles));
        DamageState();
        if (isFinish)
        {
            yield return new WaitForSeconds(mTime + 0.2f);
            GameObject slash2 = Instantiate(obj, mPos + obj.transform.position, Quaternion.Euler(new Vector3(0.0f,90.0f,-20.0f)));
            Destroy(slash2,1.0f);
            DamageState();
        }
        yield return new WaitForSeconds(0.25f);
        Destroy(slash);
        
        boss.mAnimator.SetBool("Attack2", false);
        isCompleted = true;
    }

    void DamageState()
    {
        var boss = GetComponent<Boss>();
        GameObject obj = ResourceManager.GetResource<GameObject>("Prefabs/Effects/Temple_Guardian_Explosion");
        if (boss.mBuffNerfController.GetBuffCount() > 0)
        {
            GameObject slash = Instantiate(obj, boss.mTarget.transform.position +new Vector3(0.0f,0.5f,0.0f), Quaternion.Euler(obj.transform.eulerAngles));
            Destroy(slash,1.0f);
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mShakeTime));
        }
        if (boss.mTarget)
        {
            if(boss.mAttackClips.Count() > 0)
                AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip,0.6f);
            boss.mTarget.TakeDamage(boss.mStatus.mDamage + boss.mBonusStatus.mDamage, DamageType.Physical);
        }
    }

    protected override void StartActionTrigger()
    {
        var boss = GetComponent<Boss>();
        boss.mAnimator.Play("Attack");
        if (UnityEngine.Random.Range(0, 100) >= mAttackTriggerPercentage)
        {
            isFinish = true;
            boss.mAnimator.SetBool("Attack2", true);
            mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f);
        }
        else
        {
            isFinish = false;
            mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f) / 2.0f;
        }
        mPos = boss.mTarget.transform.position;
        boss.mAiBuild.SetActionEvent(AIBuild.ActionEvent.Busy);
        isCompleted = false;
        StartCoroutine(Action());
    }

    private void OnDestroy()
    {
        GetComponent<Boss>().mActionTrigger -= StartActionTrigger;
    }
}
