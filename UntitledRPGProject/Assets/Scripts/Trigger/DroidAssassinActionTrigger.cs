using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroidAssassinActionTrigger : ActionTrigger
{
    protected override IEnumerator Action()
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(Slash());
        for (int i = 0; i < 2; ++i)
        {
            float firstMirror = Random.Range(-3.5f, 3.5f);
            GameObject mirror = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorDroidAssassin"), new Vector3(GetComponent<Unit>().mTarget.transform.position.x, GetComponent<Unit>().mTarget.transform.position.y, GetComponent<Unit>().mTarget.transform.position.z + firstMirror), Quaternion.identity);
            if(firstMirror < 0.0f)
                mirror.GetComponent<SpriteRenderer>().flipX = true;

            mirror.GetComponent<Animator>().SetTrigger("Attack1");
            mirror.GetComponent<Animator>().speed = 0.8f;
            Destroy(mirror, 0.8f);
            float secondMirror = Random.Range(-1.0f, 1.0f);

            GameObject mirror2 = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/MirrorDroidAssassin"), new Vector3(GetComponent<Unit>().mTarget.transform.position.x, GetComponent<Unit>().mTarget.transform.position.y, GetComponent<Unit>().mTarget.transform.position.z + secondMirror), Quaternion.identity);
            if (secondMirror < 0.0f)
                mirror2.GetComponent<SpriteRenderer>().flipX = true;
            mirror2.GetComponent<Animator>().SetTrigger("Attack2");
            mirror2.GetComponent<Animator>().speed = 0.8f;
            Destroy(mirror2, 0.8f);
            yield return new WaitForSeconds(mTime / 4.0f);
        }
        yield return new WaitForSeconds(mTime / 1.7f);
        GetComponent<Unit>().mTarget?.TakeDamage((GetComponent<Unit>().mStatus.mDamage + GetComponent<Unit>().mBonusStatus.mDamage), DamageType.Physical);
        StartCoroutine(GetComponent<Unit>().CounterState(GetComponent<Unit>().mTarget.mStatus.mDamage));

    }

    private IEnumerator Slash()
    {
        for (int i = 0; i < 15; ++i)
        {
            GameObject slash = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Droid_Assassin_Slash"), new Vector3(GetComponent<Unit>().mTarget.transform.position.x, GetComponent<Unit>().mTarget.transform.position.y, GetComponent<Unit>().mTarget.transform.position.z), Quaternion.identity);
            slash.transform.Rotate(new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f)));
            if(Random.Range(0,1) >= 1)
            {
                slash.GetComponent<SpriteRenderer>().flipX = true;
            }
            Destroy(slash, 0.5f);
            yield return new WaitForSeconds(0.08f);
        }
    }

    protected override void StartActionTrigger()
    {
        mPos = GetComponent<Unit>().mTarget.transform.position;
        GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.Busy;
        mTime = GetComponent<Unit>().GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime+0.5f;
        Find();
        StartCoroutine(Action());
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Unit>().mActionTrigger += StartActionTrigger;

    }

    private void OnDestroy()
    {
        GetComponent<Unit>().mActionTrigger -= StartActionTrigger;
    }

    private void Find()
    {
        List<GameObject> list = new List<GameObject>((GetComponent<Unit>().mFlag == Flag.Enemy) ? PlayerController.Instance.mHeroes.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList()
            : GameManager.Instance.mEnemyProwler.mEnemySpawnGroup.Where(t => t.GetComponent<Unit>().mConditions.isDied == false).ToList());
        if (list.Count == 0) return;
        if (GetComponent<Unit>().mAiBuild.stateMachine.mPreferredTarget)
            GetComponent<Unit>().mTarget = GetComponent<Unit>().mAiBuild.stateMachine.mPreferredTarget;
        else
        {
            int index = 0;
            float maxHealth = 0.0f;
            float currentHealth = 0.0f;
            for (int i = 0; i < list.Count; ++i)
            {
                currentHealth = list[i].GetComponent<Unit>().mStatus.mHealth;
                if (currentHealth > maxHealth)
                {
                    maxHealth = currentHealth;
                    index = i;
                }
            }
            GetComponent<Unit>().mTarget = list[index].GetComponent<Unit>();
        }
    }
}
