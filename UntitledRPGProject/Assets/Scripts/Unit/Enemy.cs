using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public StateMachine mStateMachine;

    protected override void Start()
    {
        base.Start();
        mRigidbody = GetComponent<Rigidbody>();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Enemy");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
            }
        }
        
        mStateMachine = gameObject.AddComponent<StateMachine>();
        mStateMachine.mAgent = this.gameObject;
        //TODO Add behaviors
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mRigidbody == null)
            return;
        mRigidbody.velocity = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;
    }
}
