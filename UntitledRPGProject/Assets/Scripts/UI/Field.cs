using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Field : MonoBehaviour
{
    [HideInInspector]
    public bool IsExist = false;

    private GameObject mTargetFriendly;
    private GameObject mTargetHostile;
    private GameObject mTargetMagicHostile;

    private Unit _Unit;
    public Unit mUnit { 
        set
        {
            _Unit = value;
            IsExist = true;
        } get 
        { 
            return _Unit; 
        } 
    }

    private void Start()
    {
        if(transform.GetComponent<ParticleSystem>())
            transform.GetComponent<ParticleSystem>().Stop();
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f, LayerMask.GetMask("Field"));
        for(int i = 0; i < colliders.Length; ++i)
            colliders[i].GetComponent<Field>().Initialize();
    }
    public void Initialize()
    {
        transform.Find("Effect").GetComponent<ParticleSystem>()?.Stop();
        NavMeshHit navMesh = new NavMeshHit();
        if ((!Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Ground")))
            || (Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Obstacle"))
            || (Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Field")))))
        {
            transform.position = (NavMesh.SamplePosition(transform.position, out navMesh, 30.0f, -1)) ? navMesh.position : transform.position;
        }
        transform.position += new Vector3(Random.Range(-0.5f,0.5f), 1.5f, Random.Range(-0.5f, 0.5f));
        if(mTargetFriendly == null)
            mTargetFriendly = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/FriendlyField"), transform.position, Quaternion.identity, transform);
        mTargetFriendly.transform.position = transform.position - new Vector3(0.0f,1.2f,0.0f);
        mTargetFriendly.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        mTargetFriendly.SetActive(false);

        if(mTargetHostile == null)
            mTargetHostile = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HostileField"), transform.position, Quaternion.identity, transform);
        mTargetHostile.transform.position = transform.position - new Vector3(0.0f, 1.2f, 0.0f);
        mTargetHostile.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        mTargetHostile.SetActive(false);      
        
        if(mTargetMagicHostile == null)
            mTargetMagicHostile = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HostileMagicTarget"), transform.position, Quaternion.identity, transform);
        mTargetMagicHostile.transform.position = transform.position - new Vector3(0.0f, 1.2f, 0.0f);
        mTargetMagicHostile.transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        mTargetMagicHostile.SetActive(false);
    }

    public void Picked(bool active)
    {
        if(active)
            transform.Find("Effect").GetComponent<ParticleSystem>()?.Play();
        else
            transform.Find("Effect").GetComponent<ParticleSystem>()?.Stop();
    }

    public void TargetedFriendly(bool active)
    {
        mTargetFriendly.SetActive(active);
    }

    public void TargetedHostile(bool active)
    {
        mTargetHostile.SetActive(active);
    }    
    
    public void TargetedMagicHostile(bool active)
    {
        mTargetMagicHostile.SetActive(active);
    }

    public void Stop()
    {
        transform.Find("Effect").GetComponent<ParticleSystem>()?.Stop();
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
