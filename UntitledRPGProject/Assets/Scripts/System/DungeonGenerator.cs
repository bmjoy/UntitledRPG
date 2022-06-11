using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private DungeonGeneratorInfo _Info;
    private List<Cell> _Borad;

    private int _StartIndex = 0;

    void Start()
    {
        Populate();
    }

    void Generate()
    {
        for (int y = 0; y < _Info.Row; y++)
        {
            for (int x = 0; x < _Info.Column; x++)
            {
                Cell cell = _Borad[Mathf.FloorToInt(y + x * _Info.Column)];
                Room room;
                Room.RoomType type;
                if (cell.GetVisited())
                {
                    if (y == _Info.Row - 1 && x == _Info.Column - 1)
                    {
                        room = Instantiate(_Info.BossRoom
        , new Vector3(y * (_Info.Offset.x), 0.0f, -x * (_Info.Offset.y))
        , Quaternion.identity, transform).GetComponent<Room>();
                        type = Room.RoomType.None;
                    }
                    else if (y == Mathf.FloorToInt((float)_Info.Row / 2) && x ==
                        Mathf.FloorToInt((float)_Info.Column / 2))
                    {
                        room = Instantiate(_Info.MiniBossRoom
    , new Vector3(y * _Info.Offset.x, 0.0f, -x * _Info.Offset.y)
    , Quaternion.identity, transform).GetComponent<Room>();
                        type = Room.RoomType.None;
                    }
                    else if(y == 0 && x == 0)
                    {
                        room = Instantiate(_Info.PlayerRoom
, new Vector3(y * _Info.Offset.x, 0.0f, -x * _Info.Offset.y)
, Quaternion.identity, transform).GetComponent<Room>();
                        type = Room.RoomType.Player;
                    }
                    else if(y >= Mathf.FloorToInt((float)_Info.Row / 2)
                        && x >= Mathf.FloorToInt((float)_Info.Column / 2))
                    {
                        room = Instantiate(_Info.Rooms[UnityEngine.Random.Range(0, _Info.Rooms.Count - 1)]
, new Vector3(y * _Info.Offset.x, 0.0f, -x * _Info.Offset.y)
, Quaternion.identity, transform).GetComponent<Room>();
                        if (UnityEngine.Random.Range(0, 100) >= 30)
                            type = Room.RoomType.HighTierMonster;
                        else
                            type = (Room.RoomType)UnityEngine.Random.Range(1, 4);
                    }
                    else
                    {
                        room = Instantiate(_Info.Rooms[UnityEngine.Random.Range(0, _Info.Rooms.Count - 1)]
    , new Vector3(y * _Info.Offset.x, 0.0f, -x * _Info.Offset.y)
    , Quaternion.identity, transform).GetComponent<Room>();
                        if(UnityEngine.Random.Range(0,100) >= 30)
                            type = Room.RoomType.LowTierMonster;
                        else
                            type = (Room.RoomType)UnityEngine.Random.Range(1, 4);
                    }
                    room.ConstructRoom(cell.isOpened,type);

                    room.name += " " + type.ToString();
                }
            }
        }
        GameObject manager = Instantiate(Resources.Load<GameObject>("Prefabs/Spawners/UnitSpawnManager"), transform);
    }

    void Populate()
    {
        _Borad = new List<Cell>();
        for (int y = 0; y < _Info.Row; ++y)
        {
            for (int x = 0; x < _Info.Column; ++x)
            {
                _Borad.Add(new Cell());
            }
        }
        int currentIndex = _StartIndex;
        Stack<int> path = new Stack<int>();
        int k = 0;

        while (k < _Info.Population)
        {
            try
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
            catch(Exception ex)
            {
                Debug.Log(ex.StackTrace);
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
