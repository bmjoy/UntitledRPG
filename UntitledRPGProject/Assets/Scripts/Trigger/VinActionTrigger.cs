using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VinActionTrigger : ActionTrigger
{
    private bool _isShadow = false;
    private bool isFinish = false;
    [SerializeField]
    private AudioClip[] mHitClip;
    [SerializeField]
    private int mSlashAmount = 5;
    [SerializeField]
    private float mEverySlashTime = 0.15f;
    [SerializeField]
    private float mCriticalChance = 0.6f;
    [SerializeField]
    private AudioClip clip;
    private int maxCount = 0;
    void Start()
    {
        GetComponent<Skill_DataBase>().mSkill.mActionTrigger += StartActionTrigger;
        GetComponent<Unit>().mActionTrigger += StartFinisherTrigger;
    }
    protected override void StartActionTrigger()
    {
        var unit = GetComponent<Unit>();
        mPos = transform.position;
        unit.mAiBuild.actionEvent = ActionEvent.Busy;
        _isShadow = true;
        isCompleted = false;
        for (int y = 0; y < 4; ++y)
        {
            Transform t = BattleManager.enemyFieldParent.GetChild(y);
            if (t.GetComponent<Field>().IsExist == true)
                maxCount++;
        }
        mTime = GetComponent<Skill_DataBase>().mSkill.mEffectTime;
        StartCoroutine(Action());
    }

    private void StartFinisherTrigger()
    {
        var unit = GetComponent<Unit>();
        mPos = unit.mTarget.transform.position;
        unit.mAiBuild.actionEvent = ActionEvent.Busy;
        isCompleted = false;
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
        StartCoroutine(AttackAction());
    }

    private IEnumerator AttackAction()
    {
        var unit = GetComponent<Unit>();
        if (isFinish)
        {
            GameObject obj = Resources.Load<GameObject>("Prefabs/Effects/Vin_Shadow");
            GameObject shadow = Instantiate(obj, unit.transform.position, Quaternion.Euler(obj.transform.eulerAngles));
            Destroy(shadow, 3.0f);
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < 14; ++i)
            {
                GameObject obj2 = Resources.Load<GameObject>("Prefabs/Effects/Vin_Slash");
                GameObject slash = Instantiate(obj2, mPos + new Vector3(0.0f, 1.2f, 1.0f), Quaternion.Euler(obj2.transform.eulerAngles + new Vector3(Random.Range(-180,180), Random.Range(-90, 90), Random.Range(-45, 45))));
                Destroy(slash, 3.0f);
                GameObject obj3 = Resources.Load<GameObject>("Prefabs/Effects/Vin_Blood");
                GameObject blood = Instantiate(obj3, mPos + new Vector3(Random.Range(-3f, 3f), Random.Range(0f,2f), Random.Range(-3f, 3f)), Quaternion.Euler(obj3.transform.eulerAngles));
                Destroy(blood, 3.0f);

                if (unit.mAttackClips.Count > 0)
                    AudioManager.PlaySfx(unit.mAttackClips[Random.Range(0, unit.mAttackClips.Count)].Clip);
                yield return new WaitForSeconds(Random.Range(0.05f,0.08f));
            }
            yield return new WaitForSeconds(0.3f);
            obj = Resources.Load<GameObject>("Prefabs/Effects/Vin_Shadow");
            GameObject shadow2 = Instantiate(obj, unit.transform.position, Quaternion.Euler(obj.transform.eulerAngles));
            Destroy(shadow2, 3.0f);
            isCompleted = true;
            yield return new WaitForSeconds(0.7f);
            AudioManager.PlaySfx(mHitClip[1]);
            unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage), DamageType.Physical);
            StartCoroutine(CameraSwitcher.Instance.ShakeCamera(1.0f));

        }
        else
        {
            unit.mTarget?.TakeDamage((unit.mStatus.mDamage + unit.mBonusStatus.mDamage), DamageType.Physical);
            if (unit.mAttackClips.Count > 0)
                AudioManager.PlaySfx(unit.mAttackClips[Random.Range(0, unit.mAttackClips.Count)].Clip);
            yield return new WaitForSeconds(unit.mAttackTime);
            isCompleted = true;
        }

    }

    private void Update()
    {
        CameraSwitcher.Instance.mLiftGammaGain.gamma.value = (_isShadow) ? Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, new Vector4(1, 1, 1, -0.6f), Time.deltaTime * 5.0f)
            : Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, Vector4.zero, Time.deltaTime * 5.0f);
    }

    private IEnumerator Slash()
    {
        for (int i = 0; i < mSlashAmount; ++i)
        {
            if(GetComponent<Unit>().mSkillClips.Count > 0)
                AudioManager.PlaySfx(GetComponent<Unit>().mSkillClips[Random.Range(0, GetComponent<Unit>().mSkillClips.Count - 1)].Clip);
            yield return new WaitForSeconds(mEverySlashTime);
        }
    }
    protected override IEnumerator Action()
    {
        GameObject obj = Resources.Load<GameObject>("Prefabs/Effects/MirrorVin");
        GameObject mirror = new GameObject("Mirror");
        GameObject mirror2 = new GameObject("Mirror");
        GameObject mirror3 = new GameObject("Mirror");
        GameObject mirror4 = new GameObject("Mirror");

        mirror.AddComponent<Mirror>().Initialize(GetComponent<Unit>(), transform.position);
        mirror2.AddComponent<Mirror>().Initialize(GetComponent<Unit>(), transform.position);
        mirror3.AddComponent<Mirror>().Initialize(GetComponent<Unit>(), transform.position);
        mirror4.AddComponent<Mirror>().Initialize(GetComponent<Unit>(), transform.position);

        mirror.GetComponent<Mirror>().SetTrigger("Skill");
        mirror2.GetComponent<Mirror>().SetTrigger("Skill");
        mirror3.GetComponent<Mirror>().SetTrigger("Skill");
        mirror4.GetComponent<Mirror>().SetTrigger("Skill");

        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.GetComponent<Rigidbody>().useGravity = false;
        transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * 8.0f,ForceMode.Impulse);

        yield return new WaitForSeconds(mTime / 6.0f);

        mirror.GetComponent<Animator>().speed = 0.75f;
        mirror2.GetComponent<Animator>().speed = 0.95f;
        mirror3.GetComponent<Animator>().speed = 1.15f;
        mirror4.GetComponent<Animator>().speed = 1.35f;

        transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.GetComponent<Rigidbody>().useGravity = true;
        float i = 0.0f;
        int index = 0;
        
        for (int z = 0; z < 4; ++z)
        {
            Transform t = BattleManager.enemyFieldParent.GetChild(z);
            if (t.GetComponent<Field>().IsExist == false)
            {
                if (index == maxCount)
                    index = 0;
                transform.position = BattleManager.enemyFieldParent.GetChild(index).position - new Vector3(0.0f, 0.0f, 3.0f);
            }
            else
                transform.position = t.position - new Vector3(0.0f, 0.0f, 3.0f);
            GameObject mirrors = Instantiate(obj, transform.position, Quaternion.identity);
            mirrors.GetComponent<SpriteRenderer>().flipX = transform.GetComponent<SpriteRenderer>().flipX;
            mirrors.GetComponent<Animator>().SetTrigger("Skill");
            GameObject mirrors2 = Instantiate(obj, transform.position, Quaternion.identity);
            mirrors2.GetComponent<SpriteRenderer>().flipX = transform.GetComponent<SpriteRenderer>().flipX;
            mirrors2.GetComponent<Animator>().SetTrigger("Skill");
            mirrors.GetComponent<Animator>().speed = 1.25f + i;
            Destroy(mirrors, 5.0f - (i + 1.25f));
            mirrors2.GetComponent<Animator>().speed = 1.25f + i;
            Destroy(mirrors2, 5.0f - (i + 1.25f));
            StartCoroutine(Slash());
            yield return new WaitForSeconds(mTime / 6.0f);
            i++;
            index++;
        }

        transform.position = mPos;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.None;

        Destroy(mirror);
        Destroy(mirror2);
        Destroy(mirror3);
        Destroy(mirror4);

        IEnumerable<GameObject> group = (GetComponent<Unit>().mFlag == Flag.Enemy) ? PlayerController.Instance.mHeroes : BattleManager.Instance.mEnemies;
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject unit in group)
        {
            var u = unit.GetComponent<Unit>();
            AudioManager.PlaySfx(clip);
            if (Random.Range(0.0f,1.0f) >= mCriticalChance)
                u.TakeDamage((GetComponent<Unit>().mStatus.mDamage + GetComponent<Unit>().mBonusStatus.mDamage) * 2.0f, DamageType.Physical);
            else
                u.TakeDamage((GetComponent<Unit>().mStatus.mDamage + GetComponent<Unit>().mBonusStatus.mDamage), DamageType.Physical);
        }
        yield return new WaitForSeconds(0.5f);
        _isShadow = false;
        isCompleted = true;
        yield break;
    }

    private void OnDestroy()
    {
        if (GetComponent<Skill_DataBase>().mSkill != null)
            GetComponent<Skill_DataBase>().mSkill.mActionTrigger -= StartActionTrigger;
        if (GetComponent<Unit>().mActionTrigger != null)
            GetComponent<Unit>().mActionTrigger -= StartFinisherTrigger;
    }
    private void OnApplicationQuit()
    {
        if (GetComponent<Skill_DataBase>().mSkill != null)
            GetComponent<Skill_DataBase>().mSkill.mActionTrigger -= StartActionTrigger;
        if (GetComponent<Unit>().mActionTrigger != null)
            GetComponent<Unit>().mActionTrigger -= StartFinisherTrigger;
    }
}
