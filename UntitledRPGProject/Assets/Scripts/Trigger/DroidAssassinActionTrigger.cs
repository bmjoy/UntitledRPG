using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroidAssassinActionTrigger : ActionTrigger
{
    protected override IEnumerator Action()
    {
        var unit = GetComponent<Unit>();
        float rand = 0.0f;
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(Slash());
        for (int i = 0; i < 2; ++i)
        {
            rand = Random.Range(-3.5f, 3.5f);
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorDroidAssassin"), unit.mTarget.transform.position + new Vector3(0.0f,0.0f, rand), Quaternion.identity);
            if(rand < 0.0f)
                mirror.GetComponent<SpriteRenderer>().flipX = (CameraSwitcher.isCollided)? false : true;

            mirror.GetComponent<Animator>().SetTrigger("Attack1");
            mirror.GetComponent<Animator>().speed = 0.8f;
            Destroy(mirror, 0.8f);
            rand = Random.Range(-1.0f, 1.0f);

            GameObject mirror2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorDroidAssassin"), unit.mTarget.transform.position + new Vector3(0.0f,0.0f,rand), Quaternion.identity);
            if (rand < 0.0f)
                mirror2.GetComponent<SpriteRenderer>().flipX = (CameraSwitcher.isCollided) ? false : true;
            mirror2.GetComponent<Animator>().SetTrigger("Attack2");
            mirror2.GetComponent<Animator>().speed = 0.8f;
            Destroy(mirror2, 0.8f);
            yield return new WaitForSeconds(mTime / 4.0f);
        }
        yield return new WaitForSeconds(mTime / 2.25f);
        unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage), DamageType.Physical);
        isCompleted = true;
        yield break;
    }

    private IEnumerator Slash()
    {
        var unit = GetComponent<Unit>();
        for (int i = 0; i < 15; ++i)
        {
            GameObject slash = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Droid_Assassin_Slash"), unit.mTarget.transform.position, Quaternion.identity);
            slash.transform.Rotate(new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f)));
            Destroy(slash, 0.5f);
            if (unit.mAttackClips.Count > 0)
                AudioManager.PlaySfx(GetComponent<Unit>().mAttackClips[Random.Range(0, unit.mAttackClips.Count - 1)].Clip);
            yield return new WaitForSeconds(0.08f);
        }
    }

    protected override void StartActionTrigger()
    {
        var unit = GetComponent<Unit>();
        unit.mAiBuild.priority = AITargetPriority.AimToHighHealth;
        unit.mAiBuild.SetActionEvent(ActionEvent.Busy);
        mPos = unit.mTarget.transform.position;

        unit.mAnimator.Play("Attack");
        mTime = unit.mAnimator.GetCurrentAnimatorStateInfo(0).length + 0.75f;
        isCompleted = false;
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
