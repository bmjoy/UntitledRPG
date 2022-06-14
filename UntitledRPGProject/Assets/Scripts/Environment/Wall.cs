using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wall : Environment
{
    void Start()
    {
        gameObject.AddComponent<NavMeshObstacle>().size = gameObject.GetComponent<BoxCollider>().size;
        gameObject.GetComponent<BoxCollider>().material = Resources.Load<PhysicMaterial>("Physics/ProwlerPhysics");
    }
}
