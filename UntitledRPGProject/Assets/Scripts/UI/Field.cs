using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Field : MonoBehaviour
{
    public bool IsExist = false;
    private void Start()
    {
        if(transform.GetComponent<ParticleSystem>())
            transform.GetComponent<ParticleSystem>().Stop();
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f, LayerMask.GetMask("Field"));
        for(int i = 0; i < colliders.Length; ++i)
        {
            colliders[i].GetComponent<Field>().Initialize();
        }
    }
    public void Initialize()
    {
        transform.GetComponent<ParticleSystem>()?.Stop();
        NavMeshHit navMesh = new NavMeshHit();
        if (!Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Ground")))
        {
            transform.position = (NavMesh.SamplePosition(transform.position, out navMesh, 30.0f, -1)) ? navMesh.position : transform.position;
        }
        if (Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Obstacle")))
        {
            transform.position = (NavMesh.SamplePosition(transform.position, out navMesh, 30.0f, -1)) ? navMesh.position : transform.position;
        }
        if (Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Field")))
        {
            transform.position = (NavMesh.SamplePosition(transform.position, out navMesh, 30.0f, -1)) ? navMesh.position : transform.position;
        }
        transform.position += new Vector3(Random.Range(-0.5f,0.5f), 1.5f, Random.Range(-0.5f, 0.5f));
    }

    public void Picked(bool active)
    {
        if(active)
            transform.GetComponent<ParticleSystem>()?.Play();
        else
            transform.GetComponent<ParticleSystem>()?.Stop();
    }

    public void Stop()
    {
        transform.GetComponent<ParticleSystem>()?.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = ((Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Ground")))) ? Color.green : Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }

    private void OnEnable()
    {
        if (transform.GetComponent<ParticleSystem>())
            transform.GetComponent<ParticleSystem>().Stop();
    }

    private void OnDisable()
    {
        if (transform.GetComponent<ParticleSystem>())
            transform.GetComponent<ParticleSystem>().Stop();
    }
}
