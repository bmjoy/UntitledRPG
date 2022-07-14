using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon Info")]
public class DungeonGeneratorInfo : ScriptableObject
{
    public readonly int Population = 1000;
    [Tooltip("Length")]
    public int Column;
    [Tooltip("Length")]
    public int Row;
    [Tooltip("Offset is a length between rooms")]
    public Vector2 Offset;
    [Header("Special Room Amount")]
    [Range(0, 10)] public int SecretRoomAmount = 4;
    [Range(0, 10)] public int MerchantAmount = 2;
    [Range(0, 10)] public int CompanionAmount = 2;
    [Range(0, 10)] public int RecoverAmount = 2;
    [Range(0, 10)] public int MonkAmount = 2;
    [Header("Spawn Chance Rate")]
    [Range(0.0f, 100.0f)] public float NoneRoomRate = 35.0f;
    [Range(0.0f, 100.0f)] public float HighTierMonsterChance = 40.0f;
    [Range(0.0f, 100.0f)] public float MiddleTierMonsterChance = 40.0f;
    [Range(0.0f, 100.0f)] public float LowTierMonsterChance = 40.0f;
    [Range(0.0f, 100.0f)] public float SecretRate = 20.0f;
    [Range(0.0f, 100.0f)] public float WallTrapRate = 50.0f;
    [Range(0.0f, 100.0f)] public float GroundTrapRate = 50.0f;
    [Range(0.0f, 100.0f)] public float ProjectileTrapRate = 40.0f;
    [Range(0.0f, 100.0f)] public float SwitchTrapRate = 40.0f;
    [Range(0.0f, 100.0f)] public float FireOrbRate = 20.0f;
    public bool GenerateTrapsRandom = false;

    public List<EnemyUnit> _1TierUnit = new List<EnemyUnit>(3);
    public List<EnemyUnit> _2TierUnit = new List<EnemyUnit>(3);
    public List<EnemyUnit> _3TierUnit = new List<EnemyUnit>(3);

    public List<GameObject> Rooms;
    public GameObject PlayerRoom;
    public GameObject MiniBossRoom;
    public GameObject BossRoom;
}
