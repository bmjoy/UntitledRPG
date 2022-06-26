using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : Trap
{
    [SerializeField]
    protected ParticleSystem mEffect;
    
    protected override IEnumerator Wait()
    {
        if(mEffect != null)
            mEffect.Play();
        yield return base.Wait();
        if(mEffect != null)
            mEffect.Stop();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(isHit)
        {
            GameObject damage = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Explosion")
            , PlayerController.Instance.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
            Destroy(damage, 1.5f);
        }
    }
}
