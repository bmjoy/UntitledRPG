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
    private List<AudioClip> clips = new List<AudioClip>(3);
    protected override IEnumerator Action()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        bool isHit = true;

        boss.mAnimator.SetTrigger("Skill2");
        yield return new WaitForSeconds(mTime / 6.9f);
        transform.position = BattleManager.playerFieldParent.position + new Vector3(0.0f,0.0f,-2.0f);
        foreach (Transform t in BattleManager.playerFieldParent)
        {
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorBlood"), t.position + new Vector3(0.0f, 0.0f, -0.5f), Quaternion.identity);
            mirror.GetComponent<SpriteRenderer>().flipX = transform.GetComponent<SpriteRenderer>().flipX;
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
            if (GetComponent<Unit>().mSkillClips.Count() > 0)
                AudioManager.PlaySfx(clips[1]);
        }

        IEnumerable<GameObject> group = (GetComponent<Unit>().mFlag == Flag.Player) ? BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Enemy)
: BattleManager.Instance.mUnits.Where(s => s.GetComponent<Unit>().mFlag == Flag.Player);
        DamagableAbility damagable = boss_Skill.mSkillDatas[boss_Skill.mUltimateSkillIndex] as DamagableAbility;
        foreach (GameObject unit in group)
        {
            var i = unit.GetComponent<Unit>();
            i.TakeDamage((damagable.mValue + boss.mStatus.mMagicPower + boss.mBonusStatus.mMagicPower), DamageType.Magical);
            if(isHit)
                foreach (var nerf in boss_Skill.Skill.mNerfList)
                    i.SetNerf(nerf.Initialize(boss, i));
        }

        boss.mAnimator.ResetTrigger("Skill2");
        yield return new WaitForSeconds(0.5f);
        _isRed = _isUltimate = false;
        isCompleted = true;
        foreach (GameObject mirror in _mirrors)
        {
            Destroy(mirror);
        }
        _mirrors.Clear();
        boss.mSkillClips.ElementAt(0).Clip = clips[2];
    }

    private IEnumerator Slash()
    {
        var boss = GetComponent<Boss>();
        yield return new WaitForSeconds(0.75f);
        yield return new WaitForSeconds(0.05f);
        if (boss.mAttackClips.Count() > 0)
            AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);
        yield return new WaitForSeconds(0.22f);
        if (boss.mAttackClips.Count() > 0)
            AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);
        yield return new WaitForSeconds(0.05f);
        if (boss.mAttackClips.Count() > 0)
            AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);
        for (int i = 0; i < 6; ++i)
        {
            yield return new WaitForSeconds(0.12f);
            if (boss.mAttackClips.Count() > 0)
                AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);
        }

        yield return new WaitForSeconds(0.25f);
        if (boss.mAttackClips.Count() > 0)
            AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);

        yield return new WaitForSeconds(0.25f);
        if (boss.mAttackClips.Count() > 0)
            AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);
        yield return new WaitForSeconds(0.5f);
        if (boss.mAttackClips.Count() > 0)
            AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip);
    }

    protected override void StartActionTrigger()
    {
        var boss = GetComponent<Boss>();
        var boss_Skill = GetComponent<Boss_Skill_DataBase>();
        boss.mAiBuild.SetActionEvent(AIBuild.ActionEvent.Busy);
        mTime = boss_Skill.mSkillDatas[2].mEffectTime;
        mPos = transform.position;
        _isRed = true;
        isCompleted = false;
        StartCoroutine(Action());
    }

    public void StartUltimateTrigger()
    {
        var boss = GetComponent<Boss>();
        clips[2] = boss.mSkillClips.ElementAt(0).Clip;
        boss.mSkillClips.ElementAt(0).Clip = clips[0];
        _isUltimate = true;
        _isRed = true;
        isCompleted = false;
        StartCoroutine(wait());
    }

    IEnumerator wait()
    {
        var boss = GetComponent<Boss>().GetComponent<Boss_Skill_DataBase>();
        yield return new WaitForSeconds(boss.mSkillDatas[1].mEffectTime);

        isCompleted = true;
        yield return new WaitForSeconds(1.0f);
        _isRed = false;
    }

    private IEnumerator AttackAction()
    {
        var boss = GetComponent<Boss>();
        GameObject obj = Resources.Load<GameObject>("Prefabs/Effects/BloodySlash");
        boss.mAnimator.Play(boss.MyAttackAnim[Random.Range(0,boss.MyAttackAnim.Count)]);
        mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(mTime / 3.0f);
        GameObject slash = Instantiate(obj, boss.mTarget.transform.position, Quaternion.Euler(obj.transform.eulerAngles));
        Destroy(slash, 1.0f);
        DamageState();
        yield return new WaitForSeconds(mTime / 3.0f);
        mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length / 5.0f) - 0.2f;
        DamageState();
        GameObject slash2 = Instantiate(obj, boss.mTarget.transform.position + obj.transform.position, Quaternion.Euler(new Vector3(-50.0f, 90.0f, -20.0f)));
        Destroy(slash2, 1.0f);
        yield return new WaitForSeconds(mTime);

        mTime = (boss.mAnimator.GetCurrentAnimatorStateInfo(0).length / 3.0f) - 0.2f;
        if (boss.mBuffNerfController.GetBuffCount() > 0)
        {
            GameObject slash3 = Instantiate(obj, boss.mTarget.transform.position, Quaternion.Euler(new Vector3(-50.0f, 90.0f, -20.0f)));
            Destroy(slash3, 1.0f);
            GameObject slash4 = Instantiate(obj, boss.mTarget.transform.position, Quaternion.Euler(obj.transform.eulerAngles));
            Destroy(slash4, 1.0f);
            DamageState();
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(boss.mAnimator.GetCurrentAnimatorStateInfo(0).length - 0.2f));
        }
        yield return new WaitForSeconds(0.3f);
        isCompleted = true;
    }

    private void DamageState()
    {
        var boss = GetComponent<Boss>();
        if (boss.mTarget)
        {
            boss.mTarget.TakeDamage(boss.mStatus.mDamage + boss.mBonusStatus.mDamage, DamageType.Physical);
            if (boss.mAttackClips.Count() > 0)
                AudioManager.PlaySfx(boss.mAttackClips.ElementAt(Random.Range(0, boss.mAttackClips.Count())).Clip, 0.6f);
        }
    }

    public void StartAttackActionTrigger()
    {
        var boss = GetComponent<Boss>();
        boss.mAiBuild.SetActionEvent(AIBuild.ActionEvent.Busy);
        boss.mAnimator.SetBool("Attack2", (boss.mBuffNerfController.GetBuffCount() > 0));
        mTime = boss.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        isCompleted = false;
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
        if (boss.mSkillClips.Count() > 0)
            boss.mSkillClips.ElementAt(0).Clip = clips[2];
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
        if (boss.mSkillClips.Count() > 0)
            boss.mSkillClips.ElementAt(0).Clip = clips[2];
    }
}
