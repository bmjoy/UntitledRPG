using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : MonoBehaviour
{
    List<Spawner> m_AllSpawner;
    public bool isDungeon = false;
    private void Start()
    {
        if(isDungeon == false)
        {
            m_AllSpawner = new List<Spawner>(FindObjectsOfType<Spawner>());
            SpawnAll();
        }
    }
    public void SpawnAll()
    {
        m_AllSpawner = new List<Spawner>(FindObjectsOfType<Spawner>());
        for (int i = 0; i < m_AllSpawner.Count; i++)
            m_AllSpawner[i].Spawn(isDungeon);
    }

    public void ResetSpawnAll()
    {
        m_AllSpawner = new List<Spawner>(FindObjectsOfType<Spawner>());
        for (int i = 0; i < m_AllSpawner.Count; i++)
            m_AllSpawner[i].ResetSpawn();
    }
}
