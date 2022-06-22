using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Room : Environment
{
    private Transform mDirection;
    public enum RoomType
    {
        None = 0,
        Recover,
        ArmorMerchant,
        WeaponMerchant,
        Companion,
        LowTierMonster,
        HighTierMonster,
        Secret,
        Player,
        MiniBoss,
        Boss
    }
    
    public GameObject[] mWalls;
    public GameObject[] mArcs;
    public GameObject[] mConnectors;
    public NavMeshSurface meshSurface;
    
    public void ConstructRoom(bool[] status, RoomType type)
    {
        mDirection = transform.Find("Direction");
        List<Transform> transforms = mDirection.Cast<Transform>().ToList();
        for (int i = 0; i < status.Length; ++i)
        {
            mWalls[i].SetActive(!status[i]);
            if (mConnectors.Length > 0)
            {
                if (status[i])
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), transforms[i].transform.position, Quaternion.identity);
                    go.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Door;
                    if (mConnectors[i].name.Contains("Up") || mConnectors[i].name.Contains("Down"))
                    {
                        go.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else
                    {
                        go.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
                    }
                }
                mConnectors[i].SetActive(status[i]);
            }
        }
        Vector3 pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 3.0f, GetComponent<Renderer>().bounds.size.x / 3.0f),
            0.5f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 3.0f, GetComponent<Renderer>().bounds.size.z / 3.0f));
        Spawner spawner;
        int randomAmount = 0;
        switch (type)
        {
            case RoomType.None:
                {
                    for (int i = 0; i < UnityEngine.Random.Range(0,3); i++)
                    {
                        spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                        spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Rock;
                        pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.5f, GetComponent<Renderer>().bounds.size.x / 2.5f),
            0.5f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.5f, GetComponent<Renderer>().bounds.size.z / 2.5f));
                    }

                }
                break;
            case RoomType.Recover:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Well;
                break;
            case RoomType.ArmorMerchant:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.ArmorMerchant;
                break;
            case RoomType.WeaponMerchant:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.WeaponMerchant;
                break;
            case RoomType.LowTierMonster:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnemySpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                randomAmount = Random.Range(1, 4);
                for (int i = 0; i < randomAmount; ++i)
                {
                    spawner.GetComponent<EnemySpawner>().mEnemyList.Add((EnemyUnit)Random.Range(1, 3));
                }
                break;
            case RoomType.HighTierMonster:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnemySpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                randomAmount = Random.Range(1, 4);
                for (int i = 0; i < randomAmount; ++i)
                {
                    spawner.GetComponent<EnemySpawner>().mEnemyList.Add((EnemyUnit)Random.Range(3, 7));
                }
                break;
            case RoomType.Companion:
                randomAmount = Random.Range(1, 5);
                NPCUnit unit = (NPCUnit)(randomAmount);
                if (!GameManager.Instance.IsExist(unit.ToString()))
                {
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                    spawner.GetComponent<NPCSpawner>().mType = (NPCUnit)(randomAmount);
                }
                else
                {
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                    spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Rock;
                }
                break;
            case RoomType.Secret:
                {
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                    spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Chest;
                }
                break;
            default:
                break;
        }
        meshSurface = GetComponent<NavMeshSurface>();
        meshSurface.BuildNavMesh();
    }
}
