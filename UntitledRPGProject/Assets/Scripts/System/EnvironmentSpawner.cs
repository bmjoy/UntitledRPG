using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvironmentSpawner : Spawner
{
    [SerializeField]
    private EnvironmentObject type;

    protected override GameObject CreateNewObject()
    {
        if (type == EnvironmentObject.None)
            return null;
        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Environments/" + type.ToString()),transform.position, Quaternion.identity);
        mObject.AddComponent<NavMeshObstacle>().size = mObject.GetComponent<BoxCollider>().size;
        mObject.GetComponent<Environment>().Initialize(ID);
        return mObject;
    }

    public override void Spawn()
    {
        if (mInitialized)
            return;
        ID = GameManager.s_ID++;
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f);
        mObject = CreateNewObject();
        if (mObject == null)
        {
            Debug.Log("Failed to create");
            mInitialized = false;
        }
        else
            mInitialized = true;
    }
}
