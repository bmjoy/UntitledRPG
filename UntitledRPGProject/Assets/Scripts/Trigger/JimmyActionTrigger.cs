using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimmyActionTrigger : ActionTrigger
{
    [SerializeField]
    private float mCombo = 10.0f;
    private bool isFinish = false;
    void Start()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;
        GetComponent<Skill_DataBase>().mSkill.mActionTrigger += StartSkillActionTrigger;
    }
    protected override void StartActionTrigger()
    {
        var unit = GetComponent<Unit>();
        mPos = unit.mTarget.transform.position;
        isCompleted = false;
        unit.mAiBuild.actionEvent = ActionEvent.Busy;
        if (unit.mStatus.mDamage + unit.mBonusStatus.mDamage > unit.mTarget.mStatus.mHealth || 
            GameManager.Instance.mFinisherChance >= Mathf.RoundToInt((100 * unit.mTarget.mStatus.mHealth) / unit.mTarget.mStatus.mMaxHealth))
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
    private void StartSkillActionTrigger()
    {
        mPos = transform.position;
        isCompleted = false;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        GetComponent<Unit>().mAnimator.Play("Skill");
        mTime = GetComponent<Unit>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(SkillAction());
    }

    private IEnumerator SkillAction()
    {
        var unit = GetComponent<Unit>();
        var skill = GetComponent<Skill_DataBase>().mSkill;
        Vector3 dir = (unit.mTarget.transform.position - transform.position).normalized;
        GameObject mProjectile = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HellFire"), transform.position + dir * 6.0f, Quaternion.identity);
        mProjectile.transform.localPosition += new Vector3(3.0f, transform.GetComponent<BoxCollider>().size.y + 1.0f);
        mProjectile.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        mProjectile.GetComponent<SpriteRenderer>().flipX = unit.GetComponent<SpriteRenderer>().flipX;
        Destroy(mProjectile.gameObject, unit.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + skill.mEffectTime);
        if (unit.mSkillClips.Count > 0)
            AudioManager.PlaySfx(unit.mSkillClips[UnityEngine.Random.Range(0, unit.mSkillClips.Count - 1)].Clip, 1.0f);
        yield return new WaitForSeconds(unit.mAnimator.GetCurrentAnimatorStateInfo(0).length + skill.mEffectTime);
        unit.mTarget?.TakeDamage((unit.mStatus.mMagicPower + unit.mBonusStatus.mMagicPower), DamageType.Magical);
        foreach (var nerf in skill.mNerfList)
            unit.mTarget?.SetNerf(nerf.Initialize(unit, unit.mTarget));
        isCompleted = true;
    }

    protected override IEnumerator Action()
    {
        float h = -0.60f;
        var unit = GetComponent<Unit>();
        if(isFinish)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyFinisher"), unit.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            Destroy(go, 1.0f);

            yield return new WaitForSeconds(0.8f);
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(1.0f));
            GameObject gofire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyFinishExplosion"), mPos + new Vector3(0.0f, Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f)), Quaternion.identity);
            Destroy(gofire, 1.0f);

            unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage), DamageType.Physical);
            yield return new WaitForSeconds(mTime / 2.0f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < mCombo; ++i)
            {
                GameObject gofire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyPunchFire"), unit.transform.position + new Vector3(0.0f, 0.4f + Random.Range(-0.3f, 0.1f), 1.0f), Quaternion.identity);
                GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyPunch"), new Vector3(mPos.x, mPos.y + h, mPos.z + Random.Range(-h, h)), Quaternion.identity);

                unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage) / mCombo, DamageType.Physical);
                Destroy(go, 0.5f);
                Destroy(gofire, 0.3f);

                transform.position += new Vector3(0.0f, 0.0f, Random.Range(-0.1f, 0.1f));
                GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorJimmy"), unit.transform.position, Quaternion.identity);
                mirror.GetComponent<Animator>().speed = Random.Range(0.7f, 1.05f);
                Destroy(mirror, 0.25f);
                yield return new WaitForSeconds(mTime / mCombo);
                if (unit.mAttackClips.Count > 0)
                    AudioManager.PlaySfx(unit.mAttackClips[Random.Range(0, unit.mAttackClips.Count)].Clip);
                if (unit.mTarget.mConditions.isDied)
                    break;
                h += 0.3f;
            }
        }

        isCompleted = true;
    }

    private void OnApplicationQuit()
    {
        if (GetComponent<Unit>().mActionTrigger != null)
            GetComponent<Unit>().mActionTrigger -= StartActionTrigger;
        if (GetComponent<Skill_DataBase>().mSkill.mActionTrigger != null)
            GetComponent<Skill_DataBase>().mSkill.mActionTrigger -= StartSkillActionTrigger;
    }
}
