using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    protected override void Start()
    {
        base.Start();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Player");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
            }
        }
        mAiBuild.type = AIType.Manual;
        mTarget = null;
        transform.parent = GameObject.Find("Player").gameObject.transform;
    }

    protected override void Update()
    {
        base.Update();
    }
}
