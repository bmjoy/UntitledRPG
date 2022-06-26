using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FireOrb : FireTrap
{
    [SerializeField]
    private ParticleSystem _First;
    [SerializeField]
    private ParticleSystem _Second;
    private Vector3 mDestination = Vector3.zero;
    [SerializeField]
    private float mSpeed = 2.0f;

    protected override void Start()
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle)
            return;
        base.Start();
        GetComponent<ParticleSystem>().Play();
        _First.Play();
        _Second.Play();
        mDestination = transform.position;
    }

    protected override void Update()
    {
        if (PlayerController.Instance.Interaction || PlayerController.Instance.onBattle)
            return;
        base.Update();
        transform.Rotate(new Vector3(0.0f, 15.0f, 0.0f) * Time.deltaTime * 10.0f);
        transform.position = Vector3.MoveTowards(transform.position, mDestination, Time.deltaTime * mSpeed);
    }

    protected override IEnumerator Wait()
    {
        yield return base.Wait();
        NavMeshHit mNavHit;

        bool found = NavMesh.SamplePosition(transform.position + new Vector3((float)Random.Range(-10, 10), 0.0f, (float)Random.Range(-10, 10)),
            out mNavHit, 3.0f, 3);
        mDestination = (found) ? mNavHit.position : transform.position;
    }
}
