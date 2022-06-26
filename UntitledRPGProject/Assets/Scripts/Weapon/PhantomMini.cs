using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhantomMini : Phantom
{
    private float x = 0.0f;
    private float y = 0.0f;
    private float z = 0.0f;
    bool go = false;
    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
        x = transform.position.x + UnityEngine.Random.Range(-7.0f, 7.0f);
        y = transform.position.y + UnityEngine.Random.Range(0.5f, 2.0f);
        z = transform.position.z + UnityEngine.Random.Range(-4.0f, 4.0f);

        StartCoroutine(GO());
    }

    protected override void Update()
    {
        if (isCollide == false)
        {
            if (isEffect)
                return;
            if(!go)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(x,y,z), mSpeed * Time.deltaTime * 15.0f);
            }
            else
                transform.position = Vector3.MoveTowards(transform.position, mTarget.transform.position, mSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, mTarget.transform.position) < mMaximumDistance && !isCollide)
            {
                isCollide = true;
                GameObject damageEffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/PhathomExplosion")
, mTarget.transform.position + new Vector3(UnityEngine.Random.Range(1.0f, 5.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)),
Quaternion.identity, mTarget.transform);
                Destroy(damageEffect, 1.0f);
                mActionEvent?.Invoke();
                if (clip.Length > 0)
                    AudioManager.PlaySfx(clip[UnityEngine.Random.Range(0, clip.Length - 1)]);
                Destroy(this.gameObject, 1.25f);
            }
            else
                transform.position += new Vector3(0.0f, UnityEngine.Random.Range(-0.3f, 0.3f), 0.0f);
        }
    }

    IEnumerator GO()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject Effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/PhantomExp"), transform.position, Quaternion.identity);
        Destroy(Effect, 1.0f);
        go = true;
    }
}
