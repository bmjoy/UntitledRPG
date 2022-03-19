using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    [SerializeField]
    private LayerMask mTargetMask;
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
    }

    protected override void Update()
    {
        base.Update();

        if(mOrder == Order.Standby && isPicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mTargetMask))
            {
                if(Input.GetMouseButtonDown(0))
                {
                    this.mSetting.mTarget = hit.transform.GetComponent<Unit>();
                    Debug.Log(hit.transform.name);
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                this.mSetting.mTarget = null;
                // TODO: Cancel UI
            }
        }
    }
}
