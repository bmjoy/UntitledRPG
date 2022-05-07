using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Field : MonoBehaviour
{
    public void Initialize()
    {
        if (!Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Ground")))
        {
            NavMeshHit navMesh = new NavMeshHit();
            bool check = NavMesh.SamplePosition(transform.position, out navMesh, 100.0f, -1);
            transform.position = (check) ? navMesh.position : transform.position;
            Debug.Log(check.ToString());
        }
    }
    private void OnDrawGizmos()
    {
        if (Physics.CheckSphere(transform.position, 1.0f, LayerMask.GetMask("Ground")))
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }
}
