using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimmyActionTrigger : ActionTrigger
{
    [SerializeField]
    private float mCombo = 10.0f;
    List<AudioClip> AttackClips = new List<AudioClip>();
    void Start()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;
        
        AttackClips.Add(GetComponent<Unit>().mSetting.Clips.Find(s => s.Type == SoundClip.SoundType.Attack0).Clip);
        AttackClips.Add(GetComponent<Unit>().mSetting.Clips.Find(s => s.Type == SoundClip.SoundType.Attack1).Clip);
        AttackClips.Add(GetComponent<Unit>().mSetting.Clips.Find(s => s.Type == SoundClip.SoundType.Attack2).Clip);
    }
    protected override void StartActionTrigger()
    {

        mPos = GetComponent<Unit>().mTarget.transform.position;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        mTime = GetComponent<Unit>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(Action());
    }

    protected override IEnumerator Action()
    {
        float h = -0.60f;
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < mCombo; ++i)
        {
            GameObject gofire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyPunchFire"), new Vector3(GetComponent<Unit>().transform.position.x, GetComponent<Unit>().transform.position.y + 0.4f + Random.Range(-0.3f,0.1f), GetComponent<Unit>().transform.position.z + 1.0f), Quaternion.identity);
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyPunch"), new Vector3(mPos.x, mPos.y + h, mPos.z + Random.Range(-h, h)), Quaternion.identity);

            GetComponent<Unit>().mTarget?.TakeDamage((GetComponent<Unit>().mStatus.mDamage + GetComponent<Unit>().mBonusStatus.mDamage) / mCombo, DamageType.Physical);
            StartCoroutine(GetComponent<Unit>().CounterState(GetComponent<Unit>().mTarget.mStatus.mDamage / mCombo));
            Destroy(go, 0.5f);
            Destroy(gofire, 0.3f);
            
            transform.position += new Vector3(0.0f, 0.0f, Random.Range(-0.1f, 0.1f));
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorJimmy"), new Vector3(GetComponent<Unit>().transform.position.x, GetComponent<Unit>().transform.position.y, GetComponent<Unit>().transform.position.z), Quaternion.identity);
            mirror.GetComponent<Animator>().speed = Random.Range(0.7f, 1.05f);
            Destroy(mirror, 0.25f);
            yield return new WaitForSeconds(mTime / mCombo);
            AudioManager.PlaySfx(AttackClips[Random.Range(0, 2)]);

            h += 0.3f;
        }
    }

    private void OnDestroy()
    {
        GetComponent<Unit>().mActionTrigger -= StartActionTrigger;
    }
}
