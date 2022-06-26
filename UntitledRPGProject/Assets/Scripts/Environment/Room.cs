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
    private Vector3 pos;
    public RoomType _type;
    private DungeonGeneratorInfo _info;
    private Spawner spawner;
    private int randomAmount = 0;

    public void ConstructRoom(bool[] status, RoomType type, DungeonGeneratorInfo info)
    {
        _type = type;
        _info = info;

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
                    if(Random.Range(0,100) <= info.WallTrapRate)
                    {
                        Transform t = mConnectors[i].transform;
                        List<Transform> C_Walls = t.Cast<Transform>().ToList();
                        bool left = (UnityEngine.Random.Range(0, 2) > 0) ? true : false;
                        EnvironmentSpawner WallTrapSpawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), (left) ? C_Walls[0].position : C_Walls[1].position, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                        WallTrapSpawner.type = EnvironmentObject.WallFireTrap;
                        if (mConnectors[i].name.Contains("Up"))
                        {
                            WallTrapSpawner.transform.Rotate((left) ? new Vector3(-90.0f, 0.0f, 90.0f) : new Vector3(90.0f, 0.0f, -90.0f));
                            WallTrapSpawner.transform.position += (left) ? new Vector3(-1.0f, 0.0f, 0.0f) : new Vector3(1.0f, 0.0f, 0.0f);
                        }
                        else if (mConnectors[i].name.Contains("Down"))
                        {
                            WallTrapSpawner.transform.Rotate((left) ? new Vector3(90.0f, 0.0f, 90.0f) : new Vector3(-90.0f, 0.0f, -90.0f));
                            WallTrapSpawner.transform.position += (left) ? new Vector3(-1.0f, 0.0f, 0.0f) : new Vector3(1.0f, 0.0f, 0.0f);
                        }
                        else if (mConnectors[i].name.Contains("Left"))
                        {
                            WallTrapSpawner.transform.Rotate((left) ? new Vector3(-90.0f, 0.0f, 0.0f) : new Vector3(90.0f, 0.0f, 0.0f));
                            WallTrapSpawner.transform.position += (left) ? new Vector3(0.0f, 0.0f, -1.0f) : new Vector3(0.0f, 0.0f, 1.0f);
                        }
                        else if (mConnectors[i].name.Contains("Right"))
                        {
                            WallTrapSpawner.transform.Rotate((left) ? new Vector3(-90.0f, 0.0f, 0.0f) : new Vector3(90.0f, 0.0f, 0.0f));
                            WallTrapSpawner.transform.position += (left) ? new Vector3(0.0f, 0.0f, -1.0f) : new Vector3(0.0f, 0.0f, 1.0f);
                        }
                    }
                }
                mConnectors[i].SetActive(status[i]);
            }
        }
        GenerateInterior();
        GenerateTraps();

        meshSurface = GetComponent<NavMeshSurface>();
        meshSurface.BuildNavMesh();
    }

    private void GenerateInterior()
    {
        pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 5.0f, GetComponent<Renderer>().bounds.size.x / 5.0f),
    0.5f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 5.0f, GetComponent<Renderer>().bounds.size.z / 5.0f));

        switch (_type)
        {
            case RoomType.None:
                {
                    for (int i = 0; i < UnityEngine.Random.Range(0, 4); i++)
                    {
                        spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                        spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Rock;
                        pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.5f, GetComponent<Renderer>().bounds.size.x / 2.5f),
            0.5f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.5f, GetComponent<Renderer>().bounds.size.z / 2.5f));
                    }
                }
                break;
            case RoomType.Recover:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Well;
                break;
            case RoomType.ArmorMerchant:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<NPCSpawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.ArmorMerchant;
                break;
            case RoomType.WeaponMerchant:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<NPCSpawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.WeaponMerchant;
                break;
            case RoomType.LowTierMonster:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnemySpawner"), pos, Quaternion.identity).GetComponent<EnemySpawner>();
                randomAmount = Random.Range(1, 4);
                for (int i = 0; i < randomAmount; ++i)
                {
                    spawner.GetComponent<EnemySpawner>().mEnemyList.Add((EnemyUnit)Random.Range(1, 3));
                }
                break;
            case RoomType.HighTierMonster:
                spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnemySpawner"), pos, Quaternion.identity).GetComponent<EnemySpawner>();
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
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/NPCSpawner"), pos, Quaternion.identity).GetComponent<NPCSpawner>();
                    spawner.GetComponent<NPCSpawner>().mType = (NPCUnit)(randomAmount);
                }
                else
                {
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Rock;
                }
                break;
            case RoomType.Secret:
                {
                    spawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Chest;
                }
                break;
            default:
                break;
        }
    }

    private void GenerateTraps()
    {
        if (_type != RoomType.Player)
        {
            if (Random.Range(0.0f, 100.0f) <= _info.GroundTrapRate)
            {
                for (int i = 0; i < UnityEngine.Random.Range(0, 10); i++)
                {
                    pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.25f, GetComponent<Renderer>().bounds.size.x / 2.25f),
        0.1f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.25f, GetComponent<Renderer>().bounds.size.z / 2.25f));
                    EnvironmentSpawner TrapSpawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.GroundFireTrap;
                }
            }
            if (Random.Range(0.0f, 100.0f) <= _info.SwitchTrapRate)
            {
                for (int i = 0; i < UnityEngine.Random.Range(0, 10); i++)
                {
                    pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.25f, GetComponent<Renderer>().bounds.size.x / 2.25f),
        0.1f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.25f, GetComponent<Renderer>().bounds.size.z / 2.25f));
                    EnvironmentSpawner TrapSpawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.SwitchFireTrap;
                }
            }
            if (Random.Range(0.0f, 100.0f) <= _info.FireOrbRate)
            {
                pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.25f, GetComponent<Renderer>().bounds.size.x / 2.25f),
    0.7f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.25f, GetComponent<Renderer>().bounds.size.z / 2.25f));
                EnvironmentSpawner TrapSpawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.FireOrb;
            }

            if (Random.Range(0.0f, 100.0f) <= _info.ProjectileTrapRate)
            {
                pos = transform.position + new Vector3(0.0f, 1.2f, 0.0f);
                EnvironmentSpawner TrapSpawner = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.ProjectileTrap;
                TrapSpawner.transform.Rotate(new Vector3(90.0f, 0.0f, Random.Range(-360.0f, 360.0f)));
            }
        }
    }
}
