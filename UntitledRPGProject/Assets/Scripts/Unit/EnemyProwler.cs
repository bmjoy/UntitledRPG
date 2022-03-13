using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : MonoBehaviour
{
    public NavMeshAgent mAgent;
    public int id = 0;
    [SerializeField]
    private float mDelayRemove = 3.0f;
    private float mCurrentTime = 0.0f;
    private bool isDead = false;

    [SerializeField]
    private List<Enemy> mEnemySpawnGroup;
    public List<Enemy> EnemySpawnGroup
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
        //if(EnemySpawnGroup.TrueForAll((Enemy e) => { return e.mSetting.Death; }))
        //{
        //    mCurrentTime += Time.deltaTime;
        //    if (mCurrentTime >= mDelayRemove)
        //        DestoryEnemy(id);
        //}
        // TODO: AI for Nav Mesh Agent
    }

    public void EnemySpawn(int val)
    {
        if(val == this.id)
        {
            //foreach(Enemy enemy in mEnemySpawnGroup)
            //{

            //}
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
