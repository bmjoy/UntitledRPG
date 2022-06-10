using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : Spawner
{
    [SerializeField]
    private NPCUnit mType;
    protected override GameObject CreateNewObject()
    {
        if(mObject)
        {
            Destroy(mObject);
            mObject = null;
        }
        if (mType == NPCUnit.None)
            return null;
        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Units/NPCs/" + mType.ToString() + "NPC"), transform.position, Quaternion.identity);
        mObject.AddComponent<NavMeshObstacle>().size = mObject.GetComponent<BoxCollider>().size;

        mObject.transform.position = new Vector3(transform.position.x,transform.position.y + 2.5f, transform.position.z);
        mObject.tag = "Neutral";
        mObject.layer = 9;
        //mObject.AddComponent<Prowler>().Setup(10.0f, 0.0f, 0.0f, ID, mObject);
        //mObject.GetComponent<Prowler>().Initialize();
        return mObject;
    }
}
