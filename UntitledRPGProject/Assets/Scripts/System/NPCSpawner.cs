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

        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Units/NPCs/" + mName + "NPC"), transform.position, Quaternion.identity);
        mObject.transform.position = new Vector3(transform.position.x,transform.position.y + 2.5f, transform.position.z);
        mObject.tag = "Neutral";
        mObject.layer = 9;
        mObject.AddComponent<Prowler>().Setup(10.0f, 0.0f, 0.0f, ID, mObject);
        mObject.GetComponent<Prowler>().Initialize();
        return mObject;
    }
}
