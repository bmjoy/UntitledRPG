using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : MonoBehaviour
{
    public GameObject mModel;
    public NavMeshAgent mAgent;
    private BoxCollider mCollider;
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
        mCollider = GetComponent<BoxCollider>();
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Enemy");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
            }
        }
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
            mCollider.center = new Vector3(0.0f,5.0f,0.0f);
            mCollider.enabled = false;
            mModel.SetActive(false);
            onBattle = true;
            GameObject[] fields = GameObject.FindGameObjectsWithTag("EnemyField");
            Debug.Log(fields.Length);
            GameObject[] playerFields = GameObject.FindGameObjectsWithTag("PlayerField");
            for (int i = 0; i < EnemySpawnGroup.Count; ++i)
            {
                EnemySpawnGroup[i].transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z - 2.0f);
                EnemySpawnGroup[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                EnemySpawnGroup[i].GetComponent<Unit>().yAxis = transform.localPosition.y;
                EnemySpawnGroup[i].GetComponent<Unit>().mFieldPos = fields[i].transform.position;
                EnemySpawnGroup[i].GetComponent<Unit>().SetPosition(fields[i].transform.position, playerFields[i].transform.position, ActionEvent.IntroWalk);
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
