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
        mAgent.baseOffset = 1.0f;
        mAgent.speed = (mOriginalSpeed == 0.0f) ? 1.5f : mOriginalSpeed;
        mAnimator = mModel.GetComponent<Animator>();
        mSpriteRenderer = mModel.GetComponent<SpriteRenderer>();
        //mAnimator.runtimeAnimatorController = Resources.Load("Assets/Animations/Animator/" + mModel.name) as RuntimeAnimatorController;

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
        if (BattleManager.Instance.isBattle)
            return;
        else
            mStateMachine.ActivateState();
        if((transform.eulerAngles.y) > 0 && (transform.eulerAngles.y) < 180)
            mSpriteRenderer.flipX = false;
        else
            mSpriteRenderer.flipX  = true;
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
        GameObject[] playerFields = GameObject.FindGameObjectsWithTag("PlayerField");
        Vector3 center = BattleManager.Instance.enemyCenter;
        for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            mEnemySpawnGroup[i].transform.position = new Vector3(center.x, center.y + mEnemySpawnGroup[i].GetComponent<BoxCollider>().size.y, center.z - 2.0f);
            mEnemySpawnGroup[i].GetComponent<Unit>().yAxis = fields[i].transform.localPosition.y;
            mEnemySpawnGroup[i].GetComponent<Unit>().mFieldPos = fields[i].transform.position;
            mEnemySpawnGroup[i].GetComponent<Unit>().SetPosition(fields[i].transform.position, playerFields[i].transform.position, ActionEvent.IntroWalk);
            mEnemySpawnGroup[i].gameObject.SetActive(true);
            mEnemySpawnGroup[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            mEnemySpawnGroup[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
