using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : Spawner
{
    public string mName;

    protected override GameObject CreateNewObject()
    {
        if(mObject)
        {
            Destroy(mObject);
            mObject = null;
        }
        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/" + mName + "NPC"), transform.position, Quaternion.identity);
        return mObject;
    }
}
