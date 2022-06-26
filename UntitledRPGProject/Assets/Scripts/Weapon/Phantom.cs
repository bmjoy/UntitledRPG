using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : Projectile
{
    [SerializeField]
    private int mCount;

    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize(Unit target, Action actionEvent)
    {
        mTarget = target;
        mActionEvent += actionEvent;
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(mChannelingTime / 2.0f);
        for (int i = 0; i < mCount; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>
                ("Prefabs/Bullets/PhantomArrow"), transform.position + new Vector3(0.0f,0.0f,-1.0f), Quaternion.identity);
            go.GetComponent<PhantomMini>().Initialize(mTarget, () => {
                mTarget.TakeDamage(mDamage, DamageType.Magical); });
            yield return new WaitForSeconds(0.25f);
        }
    }

    protected override void Update()
    {
        if (isCollide == false)
        {
            if (isEffect)
                return;
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
        }

    }
}
