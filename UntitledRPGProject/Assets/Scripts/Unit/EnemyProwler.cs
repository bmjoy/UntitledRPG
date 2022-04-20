using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : MonoBehaviour
{
    public GameObject mModel;
    public NavMeshAgent mAgent;
    private BoxCollider mCollider;
    private ProwlerStateMachine mStateMachine;
    public int id = 0;
    public bool onBattle = false;

    public float mRadius = 100.0f;
    public float mAngle = 60.0f;
    public Vector3 mLastPos = Vector3.zero;

    public float mOriginalSpeed = 0.0f;
    public List<GameObject> mEnemySpawnGroup;

    public void Setup(float rad, float ang, int _id, GameObject model)
    {
        mRadius = rad;
        mAngle = ang;
        id = _id;
        mModel = model;
        mEnemySpawnGroup = new List<GameObject>();
    }

    public void Initialize()
    {
        GameManager.Instance.onBattle += EnemySpawn;
        GameManager.Instance.onEnemyDeath += DestoryEnemy;
        mCollider = GetComponent<BoxCollider>();
        mCollider.isTrigger = true;
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.baseOffset = 1.0f;
        mAgent.speed = (mOriginalSpeed == 0.0f) ? 1.5f : mOriginalSpeed;
        mOriginalSpeed = mAgent.speed;
        GameObject[] agent = GameObject.FindGameObjectsWithTag("Enemy");
        if (agent.Length > 1)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), agent[i].GetComponent<Collider>());
            }
        }
        mStateMachine = gameObject.AddComponent<ProwlerStateMachine>();
        mStateMachine.mAgent = this;
        mStateMachine.AddState<Idle>(new Idle(), "Idle");
        mStateMachine.AddState<Find>(new Find(), "Find");
        mStateMachine.AddState<Pursuit>(new Pursuit(), "Pursuit");
        mStateMachine.ChangeState("Idle");
    }

    void Update()
    {
        if (BattleManager.Instance.isBattle)
            return;
        else
            mStateMachine.ActivateState();
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
            for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
            {
                mEnemySpawnGroup[i].transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z - 2.0f);
                mEnemySpawnGroup[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                mEnemySpawnGroup[i].GetComponent<Unit>().yAxis = transform.localPosition.y;
                mEnemySpawnGroup[i].GetComponent<Unit>().mFieldPos = fields[i].transform.position;
                mEnemySpawnGroup[i].GetComponent<Unit>().SetPosition(fields[i].transform.position, playerFields[i].transform.position, ActionEvent.IntroWalk);
                mEnemySpawnGroup[i].gameObject.SetActive(true);
                mEnemySpawnGroup[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    public void DestoryEnemy(int id)
    {
        if(id == this.id)
            Destroy(gameObject);
    }

    public void ChangeBehavior(string name)
    {
        mStateMachine.ChangeState(name);
    }
    private void OnDestroy()
    {
        GameManager.Instance.onBattle -= EnemySpawn;
        GameManager.Instance.onEnemyDeath -= DestoryEnemy;
    }
}
