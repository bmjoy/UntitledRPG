using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : MonoBehaviour
{
    public GameObject mModel;
    public NavMeshAgent mAgent;
    public int id = 0;
    public bool onBattle = false;

    [SerializeField]
    private List<GameObject> mEnemySpawnGroup;
    public List<GameObject> EnemySpawnGroup
    {
        get { return mEnemySpawnGroup; }
    }

    void Start()
    {
        GameManager.Instance.onBattle += EnemySpawn;
        GameManager.Instance.onEnemyDeath += DestoryEnemy;
    }

    void Update()
    {
        if (BattleManager.Instance.isBattle)
            return;
        // TODO: AI for Nav Mesh Agent
    }

    public void EnemySpawn(int val)
    {
        if(val == this.id)
        {
            mModel.SetActive(false);
            onBattle = true;
            GameObject[] fields = GameObject.FindGameObjectsWithTag("EnemyField");
            Debug.Log(fields.Length);
            GameObject[] playerFields = GameObject.FindGameObjectsWithTag("PlayerField");
            for (int i = 0; i < EnemySpawnGroup.Count; ++i)
            {
                EnemySpawnGroup[i].transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z - 2.0f);
                EnemySpawnGroup[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                EnemySpawnGroup[i].GetComponent<Unit>().SetPosition(fields[i].transform.position, playerFields[i].transform.position);
                EnemySpawnGroup[i].gameObject.SetActive(true);
                EnemySpawnGroup[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
        // TODO: Enemy spawn
    }

    public void DestoryEnemy(int id)
    {
        if(id == this.id)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.onBattle -= EnemySpawn;
        GameManager.Instance.onEnemyDeath -= DestoryEnemy;
    }
}
