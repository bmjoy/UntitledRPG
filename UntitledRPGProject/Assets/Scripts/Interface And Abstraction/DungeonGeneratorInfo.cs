using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon Info")]
public class DungeonGeneratorInfo : ScriptableObject
{
    public int Population = 1000;
    public int Column;
    public int Row;

    public int MerchantAmount = 2;
    public int CompanionAmount = 2;
    public int RecoverAmount = 2;

    public Vector2 Offset;

    public List<GameObject> Rooms;
    public GameObject PlayerRoom;
    public GameObject MiniBossRoom;
    public GameObject BossRoom;
}
