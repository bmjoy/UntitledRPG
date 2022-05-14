using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    protected GameObject mObject;
    protected bool mInitialized = false;
    protected int ID = 0;
    public Color mGizmoColor;
    protected abstract GameObject CreateNewObject();
    public virtual void Spawn()
    {
        if (mInitialized)
            return;
        ID = GameManager.s_ID++;
        mObject = CreateNewObject();
        if (mObject == null)
        {
            Debug.Log("Failed to create");
            mInitialized = false;
        }
        else
            mInitialized = true;
    }
    public virtual void ResetSpawn()
    {
        mInitialized = false;

        if (mObject != null)
            Destroy(mObject);
        Spawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = mGizmoColor;
        Gizmos.color = new Color(mGizmoColor.r, mGizmoColor.g, mGizmoColor.b,1.0f);
        Gizmos.DrawCube(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }
}
