using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBullet : Bullet
{
    private Vector3 mDirection;
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
            GameObject damage = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Trap_Projectile_Explosion")
, PlayerController.Instance.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            Destroy(damage, 1.5f);
            foreach (GameObject unit in PlayerController.Instance.mHeroes)
            {
                if (!unit.GetComponent<Unit>().mConditions.isDied)
                    unit.GetComponent<Unit>().TakeDamageByTrap(mPower);
            }
            AudioManager.PlaySfx(clip);
            Destroy(this.gameObject);
        }
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle")
            || collision.collider.gameObject.layer == LayerMask.NameToLayer("NPC")
            || collision.collider.gameObject.layer == LayerMask.NameToLayer("EnemyProwler"))
        {
            GameObject damage = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Trap_Projectile_Explosion")
, transform.position, Quaternion.identity);
            Destroy(damage, 1.5f);
            AudioManager.PlaySfx(clip);
            Destroy(this.gameObject);
        }
    }
}
