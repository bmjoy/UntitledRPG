using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvironmentSpawner : Spawner
{
    [SerializeField]
    public EnvironmentObject type;
    protected override GameObject CreateNewObject()
    {
        if (type == EnvironmentObject.None) return null;
        GameObject group = (GameObject.Find("Environments")) ? GameObject.Find("Environments").gameObject : new GameObject("Environments");
        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Environments/" + type.ToString()),transform.position, transform.rotation,group.transform);
        if (mObject.GetComponent<BoxCollider>()) mObject.AddComponent<NavMeshObstacle>().size = mObject.GetComponent<BoxCollider>().size;
        else mObject.AddComponent<NavMeshObstacle>();
        mObject.GetComponent<Environment>().Initialize(ID);
        return mObject;
    }

    public override void Spawn(bool isDungeon = false)
    {
        if (mInitialized) return;
        ID = GameManager.s_ID++;

        if(isDungeon == false)
        {
            GameObject icon = null;
            switch (type)
            {
                case EnvironmentObject.Well:
                    icon = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Icon/RecoverIcon"), transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
            if(icon != null)
                icon.transform.eulerAngles = new Vector3(90.0f, -90.0f, 0.0f);
        }

        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        mObject = CreateNewObject();
        if (mObject == null)
        {
            Debug.Log("Failed to create");
            mInitialized = false;
        }
        else
        {
            mInitialized = true;
            Destroy(this.gameObject, 2.0f);
        }
        yield return null;
    }
}
