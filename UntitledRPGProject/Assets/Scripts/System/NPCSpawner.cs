using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : Spawner
{
    [SerializeField]
    public NPCUnit mType;
    protected override GameObject CreateNewObject()
    {
        if(mObject)
        {
            Destroy(mObject);
            mObject = null;
        }
        if (mType == NPCUnit.None) return null;
        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Units/NPCs/" + mType.ToString() + "NPC"), transform.position, Quaternion.identity);
        mObject.transform.position = new Vector3(transform.position.x,transform.position.y + 1.5f, transform.position.z);
        mObject.tag = "Neutral";
        mObject.layer = 9;
        return mObject;
    }
    public override void Spawn()
    {
        if (mInitialized)
            return;
        if(PlayerController.Instance)
        {
            if (mType < (NPCUnit)5 && PlayerController.Instance.mHeroes.Exists(s => s.GetComponent<Unit>().mSetting.Name == mType.ToString()))
                return;
        }

        ID = GameManager.s_ID++;
        StartCoroutine(Wait());
        mObject = CreateNewObject();
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
