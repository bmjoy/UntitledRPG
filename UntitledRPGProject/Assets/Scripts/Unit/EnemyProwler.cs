using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : MonoBehaviour
{
    public GameObject mModel;
    public NavMeshAgent mAgent;
    public Animator mAnimator;
    private BoxCollider mCollider;
    private ProwlerStateMachine mStateMachine;
    private SpriteRenderer mSpriteRenderer;
    public int id = 0;
    public bool onBattle = false;

    public float mRadius = 100.0f;
    public float mAngle = 60.0f;
    public Vector3 mLastPos = Vector3.zero;
    public Vector3 mVelocity = Vector3.zero;

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
        mCollider = gameObject.AddComponent<BoxCollider>();
        mAgent = gameObject.AddComponent<NavMeshAgent>();
        mCollider.isTrigger = true;
        mAgent.baseOffset = 2.0f;
        mAgent.speed = (mOriginalSpeed == 0.0f) ? 1.5f : mOriginalSpeed;
        mAnimator = mModel.GetComponent<Animator>();
        mSpriteRenderer = mModel.GetComponent<SpriteRenderer>();

        mAnimator.SetFloat("Speed", 0.0f);
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
        if (BattleManager.Instance.status != BattleManager.GameStatus.None)
            return;
        else
            mStateMachine.ActivateState();
        mSpriteRenderer.flipX = (mVelocity.x < -0.1f) ? true : false;
    }

    public void EnemySpawn(int val)
    {
        if(val == this.id)
        {
            mCollider.center = new Vector3(0.0f,5.0f,0.0f);
            mCollider.enabled = false;
            mModel.SetActive(false);
            StartCoroutine(WaitForSpawn());
        }
    }

    private IEnumerator WaitForSpawn()
    {
        GameObject[] fields = GameObject.FindGameObjectsWithTag("EnemyField");
        Vector3 center = BattleManager.Instance.enemyCenter;
        for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            mEnemySpawnGroup[i].transform.position = new Vector3(center.x, center.y + mEnemySpawnGroup[i].GetComponent<BoxCollider>().size.y, center.z);
            mEnemySpawnGroup[i].GetComponent<Unit>().mField = fields[i];
            mEnemySpawnGroup[i].gameObject.SetActive(true);
        }
        onBattle = true;
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
