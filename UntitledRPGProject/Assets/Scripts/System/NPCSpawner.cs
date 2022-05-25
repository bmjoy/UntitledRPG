using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : Spawner
{
    public string mName;
    [SerializeField]
    private float mRadius = 100.0f;
    [SerializeField]
    private float mAngle = 60.0f;
    protected override GameObject CreateNewObject()
    {
        if(mObject)
        {
            Destroy(mObject);
            mObject = null;
        }

        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/" + mName + "NPC"), transform.position, Quaternion.identity);
        mObject.transform.position = new Vector3(transform.position.x,transform.position.y + 2.5f, transform.position.z);
        mObject.tag = "Neutral";
        mObject.layer = 9;
        mObject.AddComponent<Prowler>().Setup(mRadius, mAngle, ID, mObject);
        mObject.GetComponent<Prowler>().Initialize();
        return mObject;
    }
}
