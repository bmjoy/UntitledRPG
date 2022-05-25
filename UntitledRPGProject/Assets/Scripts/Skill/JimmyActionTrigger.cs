using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimmyActionTrigger : MonoBehaviour
{
    [SerializeField]
    private float mCombo = 10.0f;
    private float mTime = 0.0f;
    private Vector3 pos;
    void Start()
    {
        GetComponent<Unit>().mActionTrigger += ActionTrigger;
    }
    void ActionTrigger()
    {
        pos = GetComponent<Unit>().mTarget.transform.position;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        mTime = GetComponent<Unit>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        float h = -0.60f;
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < mCombo; ++i)
        {
            GameObject gofire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyPunchFire"), new Vector3(GetComponent<Unit>().transform.position.x, GetComponent<Unit>().transform.position.y + 0.5f + Random.Range(-0.3f,0.1f), GetComponent<Unit>().transform.position.z + 1.0f), Quaternion.identity);
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/JimmyPunch"), new Vector3(pos.x, pos.y + h, pos.z + Random.Range(-h, h)), Quaternion.identity);


            GetComponent<Unit>().mTarget?.TakeDamage((GetComponent<Unit>().mStatus.mDamage + GetComponent<Unit>().mBonusStatus.mDamage) / mCombo, DamageType.Physical);
            Destroy(go, 0.5f);
            Destroy(gofire, 0.3f);
            transform.position += new Vector3(0.0f, 0.0f, Random.Range(-0.1f, 0.1f));
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/MirrorJimmy"), new Vector3(GetComponent<Unit>().transform.position.x, GetComponent<Unit>().transform.position.y, GetComponent<Unit>().transform.position.z), Quaternion.identity);
            mirror.GetComponent<Animator>().speed = Random.Range(0.7f, 1.05f);
            Destroy(mirror, 0.25f);
            yield return new WaitForSeconds(mTime / mCombo);
            h += 0.3f;
        }
    }

    private void OnDestroy()
    {
        GetComponent<Unit>().mActionTrigger -= ActionTrigger;
    }
}
