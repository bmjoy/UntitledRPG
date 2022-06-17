using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BKActionTrigger : BossActionTrigger
{   
    bool _isRed = false;
    List<GameObject> _mirrors = new List<GameObject>();
    [SerializeField]
    private float mShakeTime = 3.0f;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioClip clip2;
    [SerializeField]
    private AudioClip originalClip;
    protected override IEnumerator Action()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        boss.mAnimator.SetTrigger("Skill2");
        yield return new WaitForSeconds(mTime / 6.9f);
        transform.position = BattleManager.playerFieldParent.position + new Vector3(0.0f,0.0f,-2.0f);
        foreach (Transform t in BattleManager.playerFieldParent)
        {
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorBlood"), t.position + new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
            mirror.GetComponent<Animator>().speed = 0.4f;
            _mirrors.Add(mirror);

        }
        StartCoroutine(Slash());
        yield return new WaitForSeconds(mTime -2.3f);
        transform.position = mPos;

        yield return new WaitForSeconds(1.2f);

        StartCoroutine(CameraSwitcher.Instance.ShakeCamera(mShakeTime));
        foreach (GameObject mirror in _mirrors)
        {
            mirror.GetComponent<Animator>().speed = 1.0f;
            mirror.GetComponent<Animator>().SetTrigger("Explosion");
            if (GetComponent<Unit>().mSkillClips.Count > 0)
                AudioManager.PlaySfx(clip2);
        }

        IEnumerable<GameObject> group = (GetComponent<Unit>().mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
: BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);
        DamagableAbility damagable = boss_Skill.mSkillDatas[boss_Skill.mUltimateSkillIndex] as DamagableAbility;
        foreach (GameObject unit in group)
        {
            var i = unit.GetComponent<Unit>();
            i.TakeDamage((damagable.mValue + boss.mStatus.mMagicPower + boss.mBonusStatus.mMagicPower), DamageType.Magical);
        }

        boss.mAnimator.ResetTrigger("Skill2");
        yield return new WaitForSeconds(0.5f);
        _isRed = false;
        _isUltimate = false;
        foreach (GameObject mirror in _mirrors)
        {
            Destroy(mirror);
        }
        _mirrors.Clear();
        boss.mSkillClips[0].Clip = originalClip;
    }

    private IEnumerator Slash()
    {
        var boss = GetComponent<Boss>();
        yield return new WaitForSeconds(0.75f);
        yield return new WaitForSeconds(0.05f);
        if (boss.mAttackClips.Count > 0)
            AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);
        yield return new WaitForSeconds(0.22f);
        if (boss.mAttackClips.Count > 0)
            AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);
        yield return new WaitForSeconds(0.05f);
        if (boss.mAttackClips.Count > 0)
            AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);
        for (int i = 0; i < 6; ++i)
        {
            yield return new WaitForSeconds(0.12f);
            if (boss.mAttackClips.Count > 0)
                AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);
        }

        yield return new WaitForSeconds(0.25f);
        if (boss.mAttackClips.Count > 0)
            AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);

        yield return new WaitForSeconds(0.25f);
        if (boss.mAttackClips.Count > 0)
            AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);
        yield return new WaitForSeconds(0.5f);
        if (boss.mAttackClips.Count > 0)
            AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip);
    }

    protected override void StartActionTrigger()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        boss.mAiBuild.SetActionEvent(ActionEvent.Busy);
        mTime = boss_Skill.mSkillDatas[2].mEffectTime;
        mPos = transform.position;
        _isRed = true;

        StartCoroutine(Action());
    }

    public void StartUltimateTrigger()
    {
        var boss = GetComponent<Boss>();
        originalClip = boss.mSkillClips[0].Clip;
        boss.mSkillClips[0].Clip = clip;
        _isUltimate = true;
    }

    private IEnumerator AttackAction()
    {
        var boss = GetComponent<Boss>();
        boss.mAnimator.Play("Attack");
        mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        yield return new WaitForSeconds(mTime / 3.0f);
        GameObject slash = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Bloody_King_Slash"), boss.mTarget.transform.position, Quaternion.identity);
        slash.GetComponent<Animator>().Play("Slash1");
        Destroy(slash, 1.0f);
        DamageState();
        yield return new WaitForSeconds(mTime / 3.0f);
        mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length / 5.0f) - 0.2f;
        DamageState();
        GameObject slash2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Bloody_King_Slash"), boss.mTarget.transform.position + new Vector3(0.0f,1.5f,0.0f), Quaternion.identity);
        slash2.GetComponent<Animator>().Play("Slash2");
        Destroy(slash2, 1.0f);
        yield return new WaitForSeconds(mTime);

        mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length / 3.0f) - 0.2f;
        if (boss.mBuffNerfController.GetBuffCount() > 0)
        {
            DamageState();
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(boss.mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
        }
    }

    private void DamageState()
    {
        var boss = GetComponent<Boss>();
        if (boss.mTarget)
        {
            boss.mTarget.TakeDamage(boss.mStatus.mDamage + boss.mBonusStatus.mDamage, DamageType.Physical);
            if (boss.mAttackClips.Count > 0)
                AudioManager.PlaySfx(boss.mAttackClips[Random.Range(0, boss.mAttackClips.Count - 1)].Clip, 0.6f);
            StartCoroutine(boss.CounterState(boss.mTarget.mStatus.mDamage));
        }
    }

    public void StartAttackActionTrigger()
    {
        var boss = GetComponent<Boss>();
        boss.mAiBuild.SetActionEvent(ActionEvent.Busy);
        mTime = boss.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        boss.mAnimator.SetBool("Attack2", (boss.mBuffNerfController.GetBuffCount() > 0));
        Debug.Log("Hi");
        StartCoroutine(AttackAction());
    }

    void Start()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        boss_Skill.mSkillDatas[2].mActionTrigger += StartActionTrigger;
        boss_Skill.mSkillDatas[1].mActionTrigger += StartUltimateTrigger;
        boss.mActionTrigger += StartAttackActionTrigger;
    }

    private void Update()
    {
        CameraSwitcher.Instance.mLiftGammaGain.gamma.value = (_isRed) ? Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, new Vector4(1, -1.0f, -1.0f, -0.5f), Time.deltaTime * 5.0f)
            : Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, Vector4.zero, Time.deltaTime * 5.0f);
    }

    private void OnDestroy()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        if (boss_Skill.mSkillDatas[2] != null)
            boss_Skill.mSkillDatas[2].mActionTrigger -= StartActionTrigger;        
        if (boss_Skill.mSkillDatas[1] != null)
            boss_Skill.mSkillDatas[1].mActionTrigger -= StartUltimateTrigger;
        if(boss.mActionTrigger != null)
            boss.mActionTrigger -= StartAttackActionTrigger;
        if (boss.mSkillClips.Count > 0)
            boss.mSkillClips[0].Clip = originalClip;
    }

    private void OnApplicationQuit()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        if (boss_Skill.mSkillDatas[2] != null)
            boss_Skill.mSkillDatas[2].mActionTrigger -= StartActionTrigger;
        if (boss_Skill.mSkillDatas[1] != null)
            boss_Skill.mSkillDatas[1].mActionTrigger -= StartUltimateTrigger;
        if (boss.mActionTrigger != null)
            boss.mActionTrigger -= StartAttackActionTrigger;
        if (boss.mSkillClips.Count > 0)
            boss.mSkillClips[0].Clip = originalClip;
    }
}
