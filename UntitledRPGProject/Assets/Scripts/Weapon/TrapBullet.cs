using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBullet : Bullet
{
    private Vector3 mDirection; 
    private float maxDistance = 35.0f;
    private float minDistance = 2.0f;
    private float distance = 0.0f;
    public override void Initialize(Transform target, float power)
    {
        mPower = (power > 0) ? power : 10.0f;
        mDirection = (target.position - transform.position).normalized;
        mInitialize = true;
    }

    protected override void FixedUpdate()
    {
        if(mInitialize)
            GetComponent<Rigidbody>().AddForce(mDirection * mSpeed);
        if (!isDamaged &&( PlayerController.Instance.Interaction || PlayerController.Instance.onBattle || PlayerController.Instance.IsDied))
        {
            isDamaged = true;
            GameObject damage = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/Trap_Projectile_Explosion")
, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            Destroy(damage, 1.5f);
            AudioManager.PlaySfx(clip);
            Destroy(this.gameObject);
        }
        distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle || PlayerController.Instance.IsDied)
            return;
        if (isDamaged)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            isDamaged = true;
            for (int i = 0; i < PlayerController.Instance.mHeroes.Count; ++i)
            {
                var unit = PlayerController.Instance.mHeroes[i].GetComponent<Unit>();
                if (!unit.mConditions.isDied)
                    unit.TakeDamageByTrap(mPower);
            }
        }
        Boom(distance);
    }

    private void Boom(float distance)
    {
        GameObject damage = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Effects/Trap_Projectile_Explosion")
, transform.position, Quaternion.identity);
        Destroy(damage, 1.5f);
        AudioManager.PlaySfx(clip, Mathf.Clamp01((distance - maxDistance) / (minDistance - maxDistance)));
        Destroy(gameObject);
    }
}
