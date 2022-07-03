using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VinActionTrigger : ActionTrigger
{
    private bool _isShadow = false;

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
        mTime = GetComponent<Skill_DataBase>().mSkill.mEffectTime;
    }
    protected override void StartActionTrigger()
    {
        mPos = transform.position;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        _isShadow = true;
        isCompleted = false;
        for (int y = 0; y < 4; ++y)
        {
            Transform t = BattleManager.enemyFieldParent.GetChild(y);
            if (t.GetComponent<Field>().IsExist == true)
                maxCount++;
        }
        StartCoroutine(Action());
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
    }

    private void OnDestroy()
    {
        if (GetComponent<Skill_DataBase>().mSkill != null)
            GetComponent<Skill_DataBase>().mSkill.mActionTrigger -= StartActionTrigger;
    }
    private void OnApplicationQuit()
    {
        if (GetComponent<Skill_DataBase>().mSkill != null)
            GetComponent<Skill_DataBase>().mSkill.mActionTrigger -= StartActionTrigger;
    }
}
