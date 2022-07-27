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
        Monk,
        Companion,
        LowTierMonster,
        MiddleTierMonster,
        HighTierMonster,
        Secret,
        Player,
        MiniBoss,
        Boss
    }
    
    public GameObject[] mWalls;
    public GameObject[] mConnectors;
    public NavMeshSurface meshSurface;
    private Vector3 pos;
    public RoomType _type;
    private DungeonGeneratorInfo _info;
    private Spawner spawner;
    private int randomAmount = 0;
    private GameObject mMainIcon = null;
    private GameObject mUnknown = null;
    private RoomDetector mDetector;
    [SerializeField]
    private GameObject mSpawnPoint;
    [SerializeField]
    private Color mGizmoColor = Color.black;

    private void Awake()
    {
        mDetector = transform.Find("Detector").GetComponent<RoomDetector>();
        if(!mSpawnPoint)
        {
            mSpawnPoint = new GameObject("SpawnPoint");
            mSpawnPoint.transform.position = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }    

        mUnknown = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/UnknownIcon"), transform.position, Quaternion.identity);
        mUnknown.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        mDetector.Initialize(this);
    }

    public void ConstructRoom(bool[] status, RoomType type, DungeonGeneratorInfo info)
    {
        _type = type;
        _info = info;
        mDirection = transform.Find("Direction");
        List<Transform> transforms = mDirection.Cast<Transform>().ToList();
        for (int i = 0; i < status.Length; ++i)
        {
            mWalls[i].SetActive(!status[i]);
            if (status[i])
            {
                GameObject go = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), transforms[i].transform.position, Quaternion.identity);
                go.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Door;
                go.transform.Rotate((mConnectors[i].name.Contains("Up") || mConnectors[i].name.Contains("Down")) ? new Vector3(0.0f, 90.0f, 0.0f) : new Vector3(0.0f, 180.0f, 0.0f));
                if (_info.GenerateTrapsRandom)
                {
                    if (Random.Range(0, 100) <= info.WallTrapRate)
                    {
                        Transform t = mConnectors[i].transform;
                        List<Transform> C_Walls = t.Cast<Transform>().ToList();
                        bool left = (UnityEngine.Random.Range(0, 2) > 0) ? true : false;
                        EnvironmentSpawner WallTrapSpawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), (left) ? C_Walls[0].position : C_Walls[1].position, Quaternion.identity).GetComponent<EnvironmentSpawner>();
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

            }
            mConnectors[i].SetActive(status[i]);
        }
        GenerateInterior();
        if(_info.GenerateTrapsRandom) GenerateTraps();
        meshSurface = GetComponent<NavMeshSurface>();
        meshSurface.BuildNavMesh();
    }

    private void GenerateInterior()
    {
        pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 5.0f, GetComponent<Renderer>().bounds.size.x / 5.0f),
    0.5f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 5.0f, GetComponent<Renderer>().bounds.size.z / 5.0f));
        randomAmount = Random.Range(1, 4);
        switch (_type)
        {
            case RoomType.None:
                {
                    for (int i = 0; i < UnityEngine.Random.Range(0, 4); i++)
                    {
                        spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<Spawner>();
                        spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Rock;
                        pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.5f, GetComponent<Renderer>().bounds.size.x / 2.5f),
            mSpawnPoint.transform.position.y + 0.5f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.5f, GetComponent<Renderer>().bounds.size.z / 2.5f));
                    }
                }
                break;
            case RoomType.Recover:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), mSpawnPoint.transform.position + new Vector3(0.0f, 0.25f,0.0f), Quaternion.identity).GetComponent<EnvironmentSpawner>();
                spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Well;
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/RecoverIcon"), transform.position, Quaternion.identity);
                break;
            case RoomType.ArmorMerchant:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/NPCSpawner"), mSpawnPoint.transform.position - new Vector3(17.0f,0.0f,0.0f), Quaternion.identity).GetComponent<NPCSpawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.ArmorMerchant;
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/ArmorIcon"), transform.position, Quaternion.identity);
                break;
            case RoomType.WeaponMerchant:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/NPCSpawner"), mSpawnPoint.transform.position - new Vector3(17.0f, 0.0f, 0.0f), Quaternion.identity).GetComponent<NPCSpawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.WeaponMerchant;
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/WeaponIcon"), transform.position, Quaternion.identity);
                break;
            case RoomType.Monk:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/NPCSpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<NPCSpawner>();
                spawner.GetComponent<NPCSpawner>().mType = NPCUnit.Monk;
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/MonkIcon"), transform.position, Quaternion.identity);
                break;
            case RoomType.LowTierMonster:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnemySpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<EnemySpawner>();
                for (int i = 0; i < randomAmount; ++i)
                {
                    if(_info._3TierUnit.Count == 0)
                        spawner.GetComponent<EnemySpawner>().mEnemyList.Add(EnemyUnit.Ghoul);
                    else
                    {
                        EnemyUnit randomUnit = _info._3TierUnit[Random.Range(0, _info._3TierUnit.Count)];
                        randomUnit = ((randomUnit == EnemyUnit.Summoner || randomUnit == EnemyUnit.Cyber_Shielder) && spawner.GetComponent<EnemySpawner>().mEnemyList.Contains(randomUnit)) ? EnemyUnit.Ghoul : randomUnit;
                        spawner.GetComponent<EnemySpawner>().mEnemyList.Add(randomUnit);
                    }
                }
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/NormalEnemyIcon"), transform.position, Quaternion.identity);
                spawner.name = "3-Tier Enemy";
                break;
            case RoomType.MiddleTierMonster:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnemySpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<EnemySpawner>();
                for (int i = 0; i < randomAmount; ++i)
                {
                    if(_info._2TierUnit.Count == 0)
                        spawner.GetComponent<EnemySpawner>().mEnemyList.Add(EnemyUnit.Dagger_Mush);
                    else
                    {
                        EnemyUnit randomUnit = _info._2TierUnit[Random.Range(0, _info._2TierUnit.Count)];
                        randomUnit = ((randomUnit == EnemyUnit.Summoner || randomUnit == EnemyUnit.Cyber_Shielder) && spawner.GetComponent<EnemySpawner>().mEnemyList.Contains(randomUnit)) ? EnemyUnit.Dagger_Mush : randomUnit;
                        spawner.GetComponent<EnemySpawner>().mEnemyList.Add(randomUnit);
                    }
                }
                spawner.name = "2-Tier Enemy";
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/NormalEnemyIcon"), transform.position, Quaternion.identity);
                break;
            case RoomType.HighTierMonster:
                spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnemySpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<EnemySpawner>();
                for (int i = 0; i < randomAmount; ++i)
                {
                    if(_info._1TierUnit.Count == 0)
                        spawner.GetComponent<EnemySpawner>().mEnemyList.Add(EnemyUnit.Droid_Assassin);
                    else
                    {
                        EnemyUnit randomUnit = _info._1TierUnit[Random.Range(0, _info._1TierUnit.Count)];
                        randomUnit = ((randomUnit == EnemyUnit.Summoner || randomUnit == EnemyUnit.Cyber_Shielder) && spawner.GetComponent<EnemySpawner>().mEnemyList.Contains(randomUnit)) ? EnemyUnit.Droid_Assassin : randomUnit;
                        spawner.GetComponent<EnemySpawner>().mEnemyList.Add(randomUnit);
                    }
                }
                spawner.name = "1-Tier Enemy";
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/NormalEnemyIcon"), transform.position, Quaternion.identity);
                break;
            case RoomType.Companion:
                randomAmount = Random.Range(1, 5);
                NPCUnit unit = (NPCUnit)(randomAmount);
                if (!GameManager.Instance.IsExist(unit.ToString()))
                {
                    spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/NPCSpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<NPCSpawner>();
                    spawner.GetComponent<NPCSpawner>().mType = (NPCUnit)(randomAmount);
                    mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/CompanionIcon"), transform.position, Quaternion.identity);
                }
                else
                {
                    spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Rock;
                }
                break;
            case RoomType.Secret:
                {
                    spawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), mSpawnPoint.transform.position, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    spawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.Chest;
                    mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/ScreatIcon"), transform.position, Quaternion.identity);
                }
                break;
            case RoomType.MiniBoss:
            case RoomType.Boss:
                mMainIcon = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/Icon/BossEnemyIcon"), transform.position, Quaternion.identity);
                break;
            default:
                break;
        }

        if(Random.Range(0, 100) <= 30)
        {
            NPCSpawner citizenSpawn = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/NPCSpawner"), mSpawnPoint.transform.position + new Vector3(Random.Range(-5.0f,5.0f),6.0f, Random.Range(-5.0f, 5.0f)), Quaternion.identity).GetComponent<NPCSpawner>();
            citizenSpawn.mType = NPCUnit.Citizen;
        }

        if(mMainIcon != null)
        {
            mMainIcon.transform.eulerAngles = new Vector3(90.0f, -90.0f, 0.0f);
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
                    EnvironmentSpawner TrapSpawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.GroundFireTrap;
                }
            }
            if (Random.Range(0.0f, 100.0f) <= _info.SwitchTrapRate)
            {
                for (int i = 0; i < UnityEngine.Random.Range(0, 10); i++)
                {
                    pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.25f, GetComponent<Renderer>().bounds.size.x / 2.25f),
        0.1f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.25f, GetComponent<Renderer>().bounds.size.z / 2.25f));
                    EnvironmentSpawner TrapSpawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                    TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.SwitchFireTrap;
                }
            }
            if (Random.Range(0.0f, 100.0f) <= _info.FireOrbRate)
            {
                pos = transform.position + new Vector3(Random.Range(-GetComponent<Renderer>().bounds.size.x / 2.25f, GetComponent<Renderer>().bounds.size.x / 2.25f),
    0.7f, Random.Range(-GetComponent<Renderer>().bounds.size.z / 2.25f, GetComponent<Renderer>().bounds.size.z / 2.25f));
                EnvironmentSpawner TrapSpawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.FireOrb;
            }

            if (Random.Range(0.0f, 100.0f) <= _info.ProjectileTrapRate)
            {
                pos = transform.position + new Vector3(0.0f, 1.2f, 0.0f);
                EnvironmentSpawner TrapSpawner = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Spawners/EnvironmentSpawner"), pos, Quaternion.identity).GetComponent<EnvironmentSpawner>();
                TrapSpawner.GetComponent<EnvironmentSpawner>().type = EnvironmentObject.ProjectileTrap;
                TrapSpawner.transform.Rotate(new Vector3(90.0f, 0.0f, Random.Range(-360.0f, 360.0f)));
            }
        }
    }

    public void Display()
    {
        Destroy(mUnknown);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = mGizmoColor;
        Gizmos.color = new Color(mGizmoColor.r, mGizmoColor.g, mGizmoColor.b, 1.0f);
        if(mSpawnPoint)
            Gizmos.DrawCube(mSpawnPoint.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
    }
}
