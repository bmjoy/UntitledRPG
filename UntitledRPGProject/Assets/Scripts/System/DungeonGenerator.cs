using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private DungeonGeneratorInfo _Info;
    private List<Cell> _Borad;
    int _CurrentWeaponMerchant = 0;
    int _CurrentArmorMerchant = 0;
    int _CurrentCompanion = 0;
    int _CurrentMonk = 0;
    int _CurrentRecover = 0;
    int _CurrentSecret = 0;

    private int _StartIndex = 0;
    private int _Count = 0;
    private int[] _SpawnOrder = new int[3];
    private int _MaxCount = 0;

    void Start()
    {
        Populate();
    }

    void Generate()
    {
        _CurrentWeaponMerchant = _CurrentArmorMerchant = _CurrentCompanion = _CurrentRecover =
            _CurrentMonk = 0;

        Room room;
        Room.RoomType type = Room.RoomType.None;
        _SpawnOrder[0] = 0;
        _SpawnOrder[1] = _MaxCount / 2;
        _SpawnOrder[2] = _MaxCount / 2 + _MaxCount / 3;

        for (int y = 0; y < _Info.Row; y++)
        {
            for (int x = 0; x < _Info.Column; x++)
            {
                Cell cell = _Borad[Mathf.FloorToInt(y + x * _Info.Column)];
                
                if (cell.GetVisited())
                {
                    room = GetRoom(y, x, out type);
                    if (type == Room.RoomType.Secret)
                        _CurrentSecret++;
                    room.ConstructRoom(cell.isOpened,type, _Info);

                    room.name += " " + type.ToString();
                }
                _Count++;
            }
        }
        UnitSpawnManager unitSpawnManager = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/UnitSpawnManager"), transform).GetComponent<UnitSpawnManager>();
        unitSpawnManager.isDungeon = true;
        unitSpawnManager.SpawnAll();
    }

    private Room GetRoom(int row, int col, out Room.RoomType type)
    {
        Vector3 pos = new Vector3(row * _Info.Offset.x, 0.0f, -col * _Info.Offset.y);
        type = Room.RoomType.None;
        if (row == 0 && col == 0)
        {
            type = Room.RoomType.Player;
            return Instantiate(_Info.PlayerRoom, pos, Quaternion.identity, transform).GetComponent<Room>();
        }
        else if(row == _Info.Row - 1 && col == _Info.Column - 1)
        {
            type = Room.RoomType.Boss;
            return Instantiate(_Info.BossRoom, pos, Quaternion.identity, transform).GetComponent<Room>();
        }
        else if(_Count == Mathf.FloorToInt((float)_MaxCount / 2.0f))
        {
            type = Room.RoomType.MiniBoss;
            return Instantiate(_Info.MiniBossRoom, pos, Quaternion.identity, transform).GetComponent<Room>();
        }        
        else if(_Count >= _SpawnOrder[2])
        {
            type = (UnityEngine.Random.Range(0.0f, 100.0f) <= _Info.SecretRate && _CurrentSecret < _Info.SecretRoomAmount) ? 
                Room.RoomType.Secret : GetRoomType(_Info.HighTierMonsterChance, Room.RoomType.HighTierMonster);
            return Instantiate(_Info.Rooms[UnityEngine.Random.Range(0, _Info.Rooms.Count)], pos, Quaternion.identity, transform).GetComponent<Room>();
        }
        else if (_Count > _SpawnOrder[1] && _Count < _SpawnOrder[2])
        {
            type = (UnityEngine.Random.Range(0.0f, 100.0f) <= _Info.SecretRate && _CurrentSecret < _Info.SecretRoomAmount) ?
                Room.RoomType.Secret : GetRoomType(_Info.MiddleTierMonsterChance, Room.RoomType.MiddleTierMonster);
            return Instantiate(_Info.Rooms[UnityEngine.Random.Range(0, _Info.Rooms.Count)], pos, Quaternion.identity, transform).GetComponent<Room>();
        }
        else
        {
            type = (UnityEngine.Random.Range(0.0f, 100.0f) <= _Info.SecretRate && _CurrentSecret < _Info.SecretRoomAmount) ?
                Room.RoomType.Secret : GetRoomType(_Info.LowTierMonsterChance, Room.RoomType.LowTierMonster);
            return Instantiate(_Info.Rooms[UnityEngine.Random.Range(0, _Info.Rooms.Count)], pos, Quaternion.identity, transform).GetComponent<Room>();
        }
    }

    private Room.RoomType GetRoomType(float Chance, Room.RoomType defaultType)
    {
        Room.RoomType type = (Room.RoomType)UnityEngine.Random.Range(0, 6);
        if (UnityEngine.Random.Range(0, 100) <= Chance)
            return defaultType;

        switch (type)
        {
            case Room.RoomType.Recover:
                if (_CurrentRecover < _Info.RecoverAmount)
                    _CurrentRecover++;
                else
                    type = defaultType;
                break;
            case Room.RoomType.ArmorMerchant:
                if (_CurrentArmorMerchant < _Info.MerchantAmount)
                    _CurrentArmorMerchant++;
                else
                    type = defaultType;
                break;
            case Room.RoomType.WeaponMerchant:
                if (_CurrentWeaponMerchant < _Info.MerchantAmount)
                    _CurrentWeaponMerchant++;
                else
                    type = defaultType;
                break;
            case Room.RoomType.Companion:
                if (_CurrentCompanion < _Info.CompanionAmount)
                    _CurrentCompanion++;
                else
                    type = defaultType;
                break;
            case Room.RoomType.Monk:
                if (_CurrentMonk < _Info.MonkAmount)
                    _CurrentMonk++;
                else
                    type = defaultType;
                break;
            case Room.RoomType.None:
                break;
        }
        return type;
    }
    void Populate()
    {
        _Borad = new List<Cell>();
        for (int y = 0; y < _Info.Row; ++y)
        {
            for (int x = 0; x < _Info.Column; ++x)
                _Borad.Add(new Cell());
        }
        _MaxCount = _Borad.Count;
        int currentIndex = _StartIndex;
        Stack<int> path = new Stack<int>();
        int k = 0;

        while (k < _Info.Population)
        {
            k++;
            _Borad[currentIndex].Visit();
            if (currentIndex == _Borad.Count - 1)
                break;
            List<int> neighbors = CheckNeighbors(currentIndex);
            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                    break;
                else
                    currentIndex = path.Pop();
            }
            else
            {
                path.Push(currentIndex);
                int newIndex = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                if (newIndex > currentIndex)
                {
                    if (newIndex - 1 == currentIndex)
                    {
                        _Borad[currentIndex].Open(2);
                        currentIndex = newIndex;
                        _Borad[currentIndex].Open(3);
                    }
                    else
                    {
                        _Borad[currentIndex].Open(1);
                        currentIndex = newIndex;
                        _Borad[currentIndex].Open(0);

                    }
                }
                else
                {
                    if (newIndex + 1 == currentIndex)
                    {
                        _Borad[currentIndex].Open(3);
                        currentIndex = newIndex;
                        _Borad[currentIndex].Open(2);
                    }
                    else
                    {
                        _Borad[currentIndex].Open(0);
                        currentIndex = newIndex;
                        _Borad[currentIndex].Open(1);
                    }
                }
            }
        }

        Generate();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();
        if(cell - _Info.Column >= 0 && !_Borad[Mathf.FloorToInt(cell-_Info.Column)].GetVisited())
            neighbors.Add(Mathf.FloorToInt(cell - _Info.Column));
        if (cell + _Info.Column < _Borad.Count && !_Borad[Mathf.FloorToInt(cell + _Info.Column)].GetVisited())
            neighbors.Add(Mathf.FloorToInt(cell + _Info.Column));
        if ((cell + 1) % _Info.Column != 0 && !_Borad[Mathf.FloorToInt(cell + 1)].GetVisited())
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        if (cell % _Info.Column != 0 && !_Borad[Mathf.FloorToInt(cell - 1)].GetVisited())
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        return neighbors;
    }
}
