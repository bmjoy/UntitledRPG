using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrap : Trap
{
    [SerializeField]
    private Transform mFirePoint;
    [SerializeField]
    private Transform mDirection;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override IEnumerator Wait()
    {
        yield return new WaitUntil(()=> isHit == true);
        mTime = 0.0f;
        isActive = isHit = mCollider.enabled = false;
        yield break;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle)
            return;
        if (other.CompareTag("Player") && isActive)
        {
            Shoot();
            isHit = true;
        }

    }

    private void Shoot()
    {
        TrapBullet projectile = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Bullets/Trap_Projectile"), mFirePoint.position, Quaternion.identity).GetComponent<TrapBullet>();
        projectile.transform.LookAt(mFirePoint);
        projectile.Initialize(mDirection, mDamage);
        GameObject Fire = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/Trap_Projectile_Fire")
, mFirePoint.position, Quaternion.identity);
        Destroy(Fire, 1.5f);
        Destroy(projectile.gameObject, 4.0f);
    }
}
