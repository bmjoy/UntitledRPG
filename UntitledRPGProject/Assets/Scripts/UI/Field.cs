using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Field : MonoBehaviour
{
    private void Start()
    {
        if(transform.GetComponent<ParticleSystem>())
            transform.GetComponent<ParticleSystem>().Stop();
    }
    public void Initialize()
    {
        transform.GetComponent<ParticleSystem>()?.Stop();
        if (!Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Ground")))
        {
            NavMeshHit navMesh = new NavMeshHit();
            transform.position = (NavMesh.SamplePosition(transform.position, out navMesh, 100.0f, -1)) ? navMesh.position : transform.position;
        }
        transform.position += new Vector3(0.0f, 1.5f, 0.0f);
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
