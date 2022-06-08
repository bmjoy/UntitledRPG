using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : MonoBehaviour
{
    List<Spawner> m_AllSpawner;

    private void Start()
    {
        m_AllSpawner = new List<Spawner>(FindObjectsOfType<Spawner>());
        SpawnAll();
    }

    public void SpawnAll()
    {
        for (int i = 0; i < m_AllSpawner.Count; i++)
            m_AllSpawner[i].Spawn();
    }

    public void ResetSpawnAll()
    {
        m_AllSpawner = new List<Spawner>(FindObjectsOfType<Spawner>());
        for (int i = 0; i < m_AllSpawner.Count; i++)
            m_AllSpawner[i].ResetSpawn();
    }
}
