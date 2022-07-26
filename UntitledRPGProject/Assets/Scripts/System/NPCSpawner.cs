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
                    for (int i = 0; i < GameManager.characterExists.Count; ++i)
                    {
                        var exist = GameManager.characterExists[i];
                        if (exist.mUnit == NPCUnit.Jimmy)
                            continue;
                        if (exist.isExist == false)
                        {
                            mType = exist.mUnit;
                            break;
                        }
                        else
                            mType = NPCUnit.None;
                    }
                    if (mType == NPCUnit.None)
                    {
                        mObject = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Environments/Rock"), transform.position, Quaternion.identity);
                        mObject.AddComponent<NavMeshObstacle>().size = mObject.GetComponent<BoxCollider>().size;
                        mObject.GetComponent<Environment>().Initialize(ID);
                        mObject.transform.Rotate(new Vector3(0.0f, Random.Range(-360.0f, 360.0f), 0.0f));
                    }
                }
                break;
            default:
                break;
        }
        if(mType != NPCUnit.None)
        {
            GameObject group = (GameObject.Find("NPCs")) ? GameObject.Find("NPCs").gameObject : new GameObject("NPCs");
            mObject = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Units/NPCs/" + mType.ToString() + "NPC"), transform.position, Quaternion.identity,group.transform);
            mObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            mObject.tag = "Neutral";
            mObject.layer = 9;

            switch (mType)
            {
                case NPCUnit.WeaponMerchant:
                case NPCUnit.ArmorMerchant:
                    mObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
                    break;
                default:
                    break;
            }
        }

        return mObject;
    }
    public override void Spawn(bool isDungeon = false)
    {
        if (mInitialized) return;
        ID = GameManager.s_ID++;

        if (isDungeon == false)
        {
            GameObject icon = null;
            switch (mType)
            {
                case NPCUnit.Eleven:
                case NPCUnit.Roger:
                case NPCUnit.Jimmy:
                case NPCUnit.Victor:
                case NPCUnit.Vin:
                    icon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/CompanionIcon"), transform.position, Quaternion.identity);
                    break;
                case NPCUnit.WeaponMerchant:
                    icon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/WeaponIcon"), transform.position, Quaternion.identity);
                    break;
                case NPCUnit.ArmorMerchant:
                    icon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/ArmorIcon"), transform.position, Quaternion.identity);
                    break;
                case NPCUnit.Monk:
                    icon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/MonkIcon"), transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
            if (icon != null)
                icon.transform.eulerAngles = new Vector3(90.0f, -90.0f, 0.0f);
        }

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
