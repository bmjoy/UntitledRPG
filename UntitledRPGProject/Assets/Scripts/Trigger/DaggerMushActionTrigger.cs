using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerMushActionTrigger : ActionTrigger
{
    protected override IEnumerator Action()
    {
        yield return new WaitForSeconds(mTime /2.0f);
        DamageState();
        yield return new WaitForSeconds(mTime/ 2.5f);
        DamageState();
    }
    void DamageState()
    {
        var unit = GetComponent<Unit>();
        if (unit.mTarget)
        {
            if (unit.mAttackClips.Count > 0)
                AudioManager.PlaySfx(unit.mAttackClips[Random.Range(0, unit.mAttackClips.Count - 1)].Clip, 0.6f);
            unit.mTarget.TakeDamage(unit.mStatus.mDamage + unit.mBonusStatus.mDamage, DamageType.Physical);
        }
    }
    protected override void StartActionTrigger()
    {
        var unit = GetComponent<Unit>();
        unit.mAnimator.Play("Attack");
        mTime = (unit.mAnimator.GetCurrentAnimatorStateInfo(0).length + 0.5f);
        mPos = unit.mTarget.transform.position;
        unit.mAiBuild.SetActionEvent(ActionEvent.Busy);
        StartCoroutine(Action());
    }

    // Update is called once per frame
    void Update()
    {
        var unit = GetComponent<Unit>();
        if (unit.mAiBuild.actionEvent == ActionEvent.AttackWalk)
            unit.mCurrentSpeed = 5.1f;
        else if (unit.mAiBuild.actionEvent == ActionEvent.BackWalk)
            unit.mCurrentSpeed = 1.0f;
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
