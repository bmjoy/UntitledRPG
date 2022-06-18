using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    protected override void Start()
    {
        base.Start();
        
        Componenet_Initialize();
        Prefab_Initialize();
        AI_Initialize();

        GameObject[] agent = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < agent.Length && agent.Length > 1; i++)
            Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
        agent = GameObject.FindGameObjectsWithTag("EnemyProwler");
        for (int i = 0; i < agent.Length && agent.Length > 1; i++)
            Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
        agent = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < agent.Length && agent.Length > 1; i++)
            Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
        mAiBuild.type = AIType.Auto;
    }
}
