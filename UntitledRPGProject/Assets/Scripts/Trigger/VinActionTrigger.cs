using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinActionTrigger : ActionTrigger
{
    private bool _isShadow = false;
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
        StartCoroutine(Action());
    }

    private void Update()
    {
        CameraSwitcher.Instance.mLiftGammaGain.gamma.value = (_isShadow) ? Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, new Vector4(1, 1, 1, -0.6f), Time.deltaTime * 5.0f)
            : Vector4.Lerp(CameraSwitcher.Instance.mLiftGammaGain.gamma.value, Vector4.zero, Time.deltaTime * 5.0f);
    }

    protected override IEnumerator Action()
    {
        GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorVin"),transform.position,Quaternion.identity);
        GameObject mirror2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorVin"),transform.position,Quaternion.identity);        
        GameObject mirror3 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorVin"),transform.position,Quaternion.identity);
        GameObject mirror4 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorVin"),transform.position,Quaternion.identity);        

        mirror.GetComponent<Animator>().SetTrigger("Skill");
        mirror2.GetComponent<Animator>().SetTrigger("Skill");
        mirror3.GetComponent<Animator>().SetTrigger("Skill");
        mirror4.GetComponent<Animator>().SetTrigger("Skill");


        transform.GetComponent<Rigidbody>().isKinematic = false;
        mirror.GetComponent<Rigidbody>().isKinematic = false;
        mirror2.GetComponent<Rigidbody>().isKinematic = false;
        mirror3.GetComponent<Rigidbody>().isKinematic = false;
        mirror4.GetComponent<Rigidbody>().isKinematic = false;

        transform.GetComponent<Rigidbody>().useGravity = false;
        mirror.GetComponent<Rigidbody>().useGravity = false;
        mirror2.GetComponent<Rigidbody>().useGravity = false;
        mirror3.GetComponent<Rigidbody>().useGravity = false;
        mirror4.GetComponent<Rigidbody>().useGravity = false;

        transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * 8.0f,ForceMode.Impulse);
        mirror.GetComponent<Rigidbody>().AddForce(Vector3.forward * 6.0f, ForceMode.Impulse);
        mirror2.GetComponent<Rigidbody>().AddForce(Vector3.forward * 4.0f, ForceMode.Impulse);
        mirror3.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2.0f, ForceMode.Impulse);
        mirror4.GetComponent<Rigidbody>().AddForce(Vector3.forward * 1.5f, ForceMode.Impulse);

        yield return new WaitForSeconds(mTime / 6.0f);
        mirror.GetComponent<Animator>().speed = 0.75f;
        mirror2.GetComponent<Animator>().speed = 0.95f;
        mirror3.GetComponent<Animator>().speed = 1.15f;
        mirror4.GetComponent<Animator>().speed = 1.35f;

        transform.GetComponent<Rigidbody>().isKinematic = true;
        mirror.GetComponent<Rigidbody>().isKinematic = true;
        mirror2.GetComponent<Rigidbody>().isKinematic = true;
        mirror3.GetComponent<Rigidbody>().isKinematic = true;
        mirror4.GetComponent<Rigidbody>().isKinematic = true;

        transform.GetComponent<Rigidbody>().useGravity = true;
        mirror.GetComponent<Rigidbody>().useGravity = true;
        mirror2.GetComponent<Rigidbody>().useGravity = true;
        mirror3.GetComponent<Rigidbody>().useGravity = true;
        mirror4.GetComponent<Rigidbody>().useGravity = true;
        float i = 0.0f;

        foreach (Transform t in BattleManager.enemyFieldParent)
        {
            transform.position = t.position - new Vector3(0.0f, 0.0f, 3.0f);
            mirror.transform.position = t.position - new Vector3(0.0f, 0.0f, 3.0f);
            mirror2.transform.position = t.position - new Vector3(0.0f, 0.0f, 3.0f);
            mirror3.transform.position = t.position - new Vector3(0.0f, 0.0f, 3.0f);
            mirror4.transform.position = t.position - new Vector3(0.0f, 0.0f, 3.0f);

            GameObject mirrors = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorVin"), transform.position, Quaternion.identity);
            mirrors.GetComponent<Animator>().SetTrigger("Skill");
            GameObject mirrors2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorVin"), transform.position, Quaternion.identity);
            mirrors2.GetComponent<Animator>().SetTrigger("Skill");
            mirrors.GetComponent<Animator>().speed = 1.25f + i;
            Destroy(mirrors, 5.0f - (i + 1.25f));
            mirrors2.GetComponent<Animator>().speed = 1.25f + i;
            Destroy(mirrors2, 5.0f - (i + 1.25f));
            yield return new WaitForSeconds(mTime / 6.0f);


            i++;
        }
        transform.position = mPos;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.None;
        Destroy(mirror);
        Destroy(mirror2);
        Destroy(mirror3);
        Destroy(mirror4);
        _isShadow = false;
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
