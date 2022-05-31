using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BKActionTrigger : ActionTrigger
{
    bool _isRed = false;
    List<GameObject> _mirrors = new List<GameObject>();
    protected override IEnumerator Action()
    {
        GetComponent<Unit>().mAnimator.SetTrigger("Skill2");
        yield return new WaitForSeconds(mTime / 6.9f);
        transform.position = BattleManager.playerFieldParent.position + new Vector3(0.0f,0.0f,-2.0f);
        foreach (Transform t in BattleManager.playerFieldParent)
        {
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorBlood"), t.position + new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
            mirror.GetComponent<Animator>().speed = 0.4f;
            _mirrors.Add(mirror);

        }
        yield return new WaitForSeconds(mTime -2.3f);

        transform.position = mPos;
        yield return new WaitForSeconds(1.2f);
        StartCoroutine(CameraSwitcher.Instance.ShakeCamera(1.0f));
        foreach (GameObject mirror in _mirrors)
        {
            mirror.GetComponent<Animator>().speed = 1.0f;
            mirror.GetComponent<Animator>().SetTrigger("Explosion");
        }
        GetComponent<Unit>().mAnimator.ResetTrigger("Skill2");


        yield return new WaitForSeconds(0.5f);
        _isRed = false;
        _isUltimate = false;
        foreach (GameObject mirror in _mirrors)
        {
            Destroy(mirror);
        }
        _mirrors.Clear();
    }

    protected override void StartActionTrigger()
    {
        GetComponent<Boss>().mAiBuild.actionEvent = ActionEvent.Busy;
        mTime = GetComponent<Boss_Skill_DataBase>().mSkillDatas[2].mEffectTime;
        mPos = transform.position;
        _isRed = true;
        StartCoroutine(Action());
    }

    public void StartUltimateTrigger()
    {
        _isUltimate = true;
    }

    private IEnumerator AttackAction()
    {
        mTime = (GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime) / 2.0f;
        GameObject slash = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Bloody_King_Slash"), GetComponent<Boss>().mTarget.transform.position, Quaternion.identity);
        slash.GetComponent<Animator>().Play("Slash1");
        Destroy(slash, 1.0f);
        StartCoroutine(Damage());
        yield return new WaitForSeconds(mTime);
        mTime = (GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).length / 5.0f) - 0.2f;

        StartCoroutine(Damage());
        yield return new WaitForSeconds(mTime);
        GameObject slash2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Bloody_King_Slash"), new Vector3(GetComponent<Boss>().mTarget.transform.position.x, GetComponent<Boss>().mTarget.transform.position.y + 1.5f , GetComponent<Boss>().mTarget.transform.position.z), Quaternion.identity);
        slash2.GetComponent<Animator>().Play("Slash2");
        Destroy(slash2, 1.0f);
        mTime = (GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).length / 3.0f) - 0.2f;
        if (GetComponent<Boss>().mBuffNerfController.GetBuffCount() > 0)
        {
            StartCoroutine(Damage());
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(GetComponent<Boss>().mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
            yield return new WaitForSeconds(mTime + 0.75f);
        }
        else
            yield return new WaitForSeconds(mTime + 0.75f);
        if (GetComponent<Boss>().mStatus.mHealth > 0.0f)
            GetComponent<Boss>().mAiBuild.actionEvent = ActionEvent.BackWalk;
        yield return new WaitUntil(() => GetComponent<Boss>().mAiBuild.actionEvent == ActionEvent.Busy);
        GetComponent<Boss>().mAnimator.SetBool("Attack2", false);
        GetComponent<Boss>().TurnEnded();

    }

    private IEnumerator Damage()
    {
        
        if (GetComponent<Boss>().mTarget)
        {
            GetComponent<Boss>().mTarget.TakeDamage(GetComponent<Boss>().mStatus.mDamage + GetComponent<Boss>().mBonusStatus.mDamage, DamageType.Physical);
            if (GetComponent<Unit>().mTarget.mBuffNerfController.SearchBuff("Counter"))
            {
                Counter counter = GetComponent<Unit>().mTarget.mBuffNerfController.GetBuff("Counter") as Counter;
                if (counter.mChanceRate >= UnityEngine.Random.Range(0.0f, 1.0f))
                {
                    yield return new WaitForSeconds(0.25f);
                    GetComponent<Unit>().mTarget.mTarget = this.GetComponent<Unit>();
                    GetComponent<Unit>().mTarget.mTarget.TakeDamage(GetComponent<Unit>().mTarget.mStatus.mDamage, DamageType.Magical);
                    if (GetComponent<Unit>().mStatus.mHealth <= 0.0f)
                    {
                        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
                    }
                }

            }
        }
    }

    public void StartAttackActionTrigger()
    {
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        mTime = GetComponent<Unit>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        GetComponent<Animator>().SetBool("Attack2", (GetComponent<Boss>().mBuffNerfController.GetBuffCount() > 0) ? true : false);
        StartCoroutine(AttackAction());
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Boss_Skill_DataBase>().mSkillDatas[2].mActionTrigger += StartActionTrigger;
        GetComponent<Boss_Skill_DataBase>().mSkillDatas[1].mActionTrigger += StartUltimateTrigger;
        GetComponent<Unit>().mActionTrigger += StartAttackActionTrigger;
    }

    private void Update()
    {
        CameraSwitcher.Instance.mLiftGammaGain.gamma.value = (_isRed) ? Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, new Vector4(1, -1.0f, -1.0f, -0.5f), Time.deltaTime * 5.0f)
            : Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, Vector4.zero, Time.deltaTime * 5.0f);
    }

    private void OnDestroy()
    {
        if (GetComponent<Boss_Skill_DataBase>().mSkillDatas[2] != null)
            GetComponent<Boss_Skill_DataBase>().mSkillDatas[2].mActionTrigger -= StartActionTrigger;        
        if (GetComponent<Boss_Skill_DataBase>().mSkillDatas[1] != null)
            GetComponent<Boss_Skill_DataBase>().mSkillDatas[1].mActionTrigger -= StartUltimateTrigger;
        GetComponent<Unit>().mActionTrigger -= StartAttackActionTrigger;
    }

    private void OnApplicationQuit()
    {
        if (GetComponent<Boss_Skill_DataBase>().mSkillDatas[2] != null)
            GetComponent<Boss_Skill_DataBase>().mSkillDatas[2].mActionTrigger -= StartActionTrigger;
        if (GetComponent<Boss_Skill_DataBase>().mSkillDatas[1] != null)
            GetComponent<Boss_Skill_DataBase>().mSkillDatas[1].mActionTrigger -= StartUltimateTrigger;
        GetComponent<Unit>().mActionTrigger -= StartAttackActionTrigger;
    }
}
