using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : Environment
{
    public enum RoomType
    {
        None = 0,
        Recover,
        ArmorMerchant,
        WeaponMerchant,
        Companion,
        LowTierMonster,
        HighTierMonster,
        Player
    }
    
    public GameObject[] mWalls;
    public GameObject[] mConnectors;
    public NavMeshSurface meshSurface;
    
    public void ConstructRoom(bool[] status, RoomType type)
    {
        for (int i = 0; i < status.Length; ++i)
        {
            mWalls[i].SetActive(!status[i]);
            if(mConnectors.Length > 0)
                mConnectors[i].SetActive(status[i]);
        }

        Vector3 pos = transform.position + new Vector3(Random.Range(-0.5f,0.5f), 0.5f, Random.Range(-0.5f, 0.5f));
        Spawner spawner;
        int randomAmount = 0;
        switch (type)
        {
            case RoomType.None:
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
                    spawner.GetComponent<EnemySpawner>().mEnemyList.Add((EnemyUnit)Random.Range(3, 5));
                }
                break;
            case RoomType.Player:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/PlayerSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                spawner.GetComponent<PlayerSpawner>().mName = "Jimmy";
                break;
            case RoomType.Companion:
                int index = Random.Range(0, 3);
                if (GameManager.Instance.CompanionCharacters[index] == false)
                {
                    GameManager.Instance.CompanionCharacters[index] = true;
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                    spawner.GetComponent<NPCSpawner>().mType = (NPCUnit)(index + 1);
                }
                break;
            default:
                break;
        }
        meshSurface = GetComponent<NavMeshSurface>();
        meshSurface.BuildNavMesh();
    }
}
