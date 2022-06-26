using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Environment
{
    protected float mTime = 0.0f;
    [SerializeField]
    protected float mEveryTimetoActive = 2.0f;
    [SerializeField]
    protected float mDamage = 5.0f;
    protected bool isActive = false;
    protected bool isHit = false;
    protected BoxCollider mCollider;

    virtual protected void Start()
    {
        mCollider = GetComponent<BoxCollider>();
    }

    virtual protected void Update()
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle)
            return;
        mTime += Time.deltaTime;
        if(mEveryTimetoActive <= mTime)
        {
            if(isActive == false)
            {
                mCollider.enabled = true;
                StartCoroutine(Wait());
            }
            isActive = true;
        }
        else
            mTime += Time.deltaTime;

    }
    virtual protected IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f);
        mTime = 0.0f;
        isActive = isHit = mCollider.enabled = false;
        yield break;
    }

    virtual protected void OnTriggerEnter(Collider other)
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle || PlayerController.Instance.IsDied)
            return;
        if(other.CompareTag("Player") && isActive)
        {
            AudioManager.PlaySfx(mSFX, 0.6f);
            foreach (GameObject unit in PlayerController.Instance.mHeroes)
            {
                if(!unit.GetComponent<Unit>().mConditions.isDied)
                    unit.GetComponent<Unit>().TakeDamageByTrap(mDamage);
            }
            isHit = true;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("NPC")
            || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            Destroy(gameObject);
    }
}
