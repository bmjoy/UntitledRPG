using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : Spawner
{
    [SerializeField]
    private EnvironmentObject type;

    protected override GameObject CreateNewObject()
    {
        if (type == EnvironmentObject.None)
            return null;
        GameObject newObject = Instantiate(Resources.Load<GameObject>("Prefabs/Environments/" + type.ToString()),transform.position, Quaternion.identity);
        newObject.GetComponent<Environment>().Initialize(ID);
        return newObject;
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
