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
        switch(mType)
        {
            case NPCUnit.Eleven:
            case NPCUnit.Roger:
            case NPCUnit.Victor:
            case NPCUnit.Vin:
                if (GameManager.Instance.IsExist(mType.ToString()))
                {
                    for (int i = 0; i < GameManager.Instance.characterExists.Count; ++i)
                    {
                        var exist = GameManager.Instance.characterExists[i];
                        if (exist.mUnit == NPCUnit.Jimmy)
                            continue;
                        if (exist.isExist == false)
                        {
                            mType = exist.mUnit;
                            GameManager.Instance.AssignCharacter(mType.ToString());
                            break;
                        }
                        else
                            mType = NPCUnit.None;
                    }
                    if (mType == NPCUnit.None)
                    {
                        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Environments/Rock"), transform.position, Quaternion.identity);
                        mObject.AddComponent<NavMeshObstacle>().size = mObject.GetComponent<BoxCollider>().size;
                        mObject.GetComponent<Environment>().Initialize(ID);
                        mObject.transform.Rotate(new Vector3(0.0f, Random.Range(-360.0f, 360.0f), 0.0f));
                    }
                }
                else
                    GameManager.Instance.AssignCharacter(mType.ToString());
                break;
            default:
                break;
        }
        if(mType != NPCUnit.None)
        {
            mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Units/NPCs/" + mType.ToString() + "NPC"), transform.position, Quaternion.identity);
            mObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            mObject.tag = "Neutral";
            mObject.layer = 9;
        }

        return mObject;
    }
    public override void Spawn()
    {
        if (mInitialized) return;
        ID = GameManager.s_ID++;
        StartCoroutine(Wait());
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f);
        mObject = CreateNewObject();
        if (mObject == null)
        {
            Debug.Log($"{mType.ToString()} Failed to create");
            mInitialized = false;
        }
        else
            mInitialized = true;
    }
}
