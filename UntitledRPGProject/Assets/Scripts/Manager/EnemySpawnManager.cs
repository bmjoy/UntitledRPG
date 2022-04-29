using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    List<EnemySpawner> m_Spawns;

    void Awake()
    {
        m_Spawns = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());
    }

    private void Start()
    {
        SpawnAll();
    }

    private void SpawnAll()
    {
        for (int i = 0; i < m_Spawns.Count; i++)
        {
            m_Spawns[i].StartSpawn();
        }
    }
}
