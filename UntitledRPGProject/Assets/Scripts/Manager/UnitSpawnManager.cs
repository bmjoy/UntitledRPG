using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : MonoBehaviour
{
    List<EnemySpawner> m_EnemySpawns;
    PlayerSpawner mPlayerSpawner;

    private void Start()
    {
        mPlayerSpawner = GameObject.FindObjectOfType<PlayerSpawner>();
        m_EnemySpawns = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());
        SpawnAll();
    }

    public void SpawnAll()
    {
        mPlayerSpawner.Respawn();

        for (int i = 0; i < m_EnemySpawns.Count; i++)
        {
            m_EnemySpawns[i].StartSpawn();
        }
    }

    public void SpawnPlayer()
    {
        mPlayerSpawner.Spawn();
    }

    public void ResetSpawnAll()
    {
        mPlayerSpawner.Respawn();
        for (int i = 0; i < m_EnemySpawns.Count; i++)
        {
            m_EnemySpawns[i].ResetSpawn();
        }
    }
}
