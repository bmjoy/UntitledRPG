using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchFireTrap : Trap
{
    [SerializeField]
    protected ParticleSystem mEffect;
    [SerializeField]
    private GameObject mPointLight;
    [SerializeField]
    protected AudioClip mSFX2;


    protected override void Start()
    {
        base.Start();
        if (mEffect != null)
            mEffect.Stop();
        if (mPointLight != null)
            mPointLight.SetActive(false);
    }

    protected override void Update()
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle || PlayerController.Instance.IsDied)
            return;
        if(isHit)
        {
            mTime += Time.deltaTime;
            if (mEveryTimetoActive <= mTime)
            {
                if (isActive == false)
                {
                    mCollider.enabled = true;
                    StartCoroutine(Wait());
                }
                isActive = true;
            }
            else
                mTime += Time.deltaTime;
        }
    }

    protected override IEnumerator Wait()
    {
        if (mEffect != null)
            mEffect.Play();
        if (mPointLight != null)
            mPointLight.SetActive(true);
        isActive = true;
        yield return new WaitForSeconds(1.0f);
        mTime = 0.0f;
        isActive = isHit = false;
        if (mEffect != null)
            mEffect.Stop();
        if (mPointLight != null)
            mPointLight.SetActive(false);
        yield break;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle || PlayerController.Instance.IsDied)
            return;
        if (other.CompareTag("Player") && !isActive && !isHit)
        {
            AudioManager.PlaySfx(mSFX, 1.0f);
            isHit = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle || PlayerController.Instance.IsDied)
            return;
        if (other.CompareTag("Player") && isHit && isActive)
        {
            AudioManager.PlaySfx(mSFX2, 0.6f);
            GameObject damage = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Explosion")
            , PlayerController.Instance.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            Destroy(damage, 1.5f);
            foreach (GameObject unit in PlayerController.Instance.mHeroes)
            {
                if (!unit.GetComponent<Unit>().mConditions.isDied)
                    unit.GetComponent<Unit>().TakeDamageByTrap(mDamage);
            }
            isHit = false;
        }
    }
}
