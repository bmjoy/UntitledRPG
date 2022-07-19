using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogerActionTrigger : ActionTrigger
{
    private bool isFinish = false;
    public AudioClip[] mClips;

    private void Start()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;
    }

    protected override void StartActionTrigger()
    {
        var unit = GetComponent<Unit>();
        mPos = unit.mTarget.transform.position;
        isCompleted = false;
        unit.mAiBuild.actionEvent = ActionEvent.Busy;
        if (unit.mStatus.mDamage + unit.mBonusStatus.mDamage > unit.mTarget.mStatus.mHealth)
        {
            isFinish = true;
            unit.mAnimator.Play("Finisher");
        }
        else
        {
            isFinish = false;
            unit.mAnimator.Play("Attack");
        }
        mTime = GetComponent<Unit>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(Action());
    }

    protected override IEnumerator Action()
    {
        var unit = GetComponent<Unit>();
        if (isFinish)
        {
            for (int i = 0; i < 8; i++)
            {
                AudioManager.PlaySfx(mClips[Random.Range(0,mClips.Length)]);
                yield return new WaitForSeconds(mTime / 10.0f);
            }
            yield return new WaitForSeconds(0.5f);
            GameObject obj = Resources.Load<GameObject>("Prefabs/Effects/Roger_Slash");
            GameObject slash = Instantiate(obj, mPos + new Vector3(0.0f,1.2f,1.0f), Quaternion.Euler(obj.transform.eulerAngles));
            Destroy(slash, 1.1f);
            if (unit.mAttackClips.Count > 0)
                AudioManager.PlaySfx(unit.mAttackClips[Random.Range(0, unit.mAttackClips.Count)].Clip);
            unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage), DamageType.Physical); 
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(1.0f));
            yield return new WaitForSeconds(0.5f);

        }
        else
        {
            unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage), DamageType.Physical);
            if (unit.mAttackClips.Count > 0)
                AudioManager.PlaySfx(unit.mAttackClips[Random.Range(0, unit.mAttackClips.Count)].Clip);
            yield return new WaitForSeconds(unit.mAttackTime);
        }
        isCompleted = true;
        yield break;
    }

    private void OnApplicationQuit()
    {
        if (GetComponent<Unit>().mActionTrigger != null)
            GetComponent<Unit>().mActionTrigger -= StartActionTrigger;
    }
}
