using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyProwler : Prowler
{
    [HideInInspector]
    public GameObject mCanvas;
    [HideInInspector]
    public GameObject mTextCanvas;
    [HideInInspector]
    public GameObject mExclamation;
    [HideInInspector]
    public GameObject mParticles;
    [HideInInspector]
    public List<GameObject> mEnemySpawnGroup;
    [HideInInspector]
    public bool isWin = false;

    private float maxDistance = 10.0f;
    private float minDistance = 2.0f;
    public override void Setup(float rad, float ang, float speed, int _id, GameObject model)
    {
        base.Setup(rad, ang, speed, _id, model);
        mEnemySpawnGroup = new List<GameObject>();
    }

    public override void Initialize(Spawner spawner = null)
    {
        GameManager.Instance.onBattle += EnemySpawn;
        GameManager.Instance.onEnemyWin += Win;

        base.Initialize(spawner);
        mCanvas = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/UI/CanvasForEnemyProwler"), transform.position
+ new Vector3(0.0f, GetComponent<BoxCollider>().center.y + 3.0f, 0.0f), Quaternion.identity, mModel.transform);

        if(mModel.transform.Find("Canvas") != null)
        {
            mTextCanvas = mModel.transform.Find("Canvas").gameObject;
            mTextCanvas.SetActive(false);
        }


        mExclamation = mCanvas.transform.Find("Exclamation").gameObject;
        mParticles = mCanvas.transform.Find("Found").gameObject;

        if (!mModel.GetComponent<Billboard>().mUseStaticBillboard)
            mCanvas.transform.localRotation = new Quaternion(0.0f, 180.0f, 0.0f, 1.0f);
        else
            mCanvas.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);


        mExclamation.SetActive(false);
        mParticles.SetActive(false);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (BattleManager.status != BattleManager.GameStatus.None || PlayerController.Instance.Interaction)
        {
            mExclamation.SetActive(false);
            mParticles.SetActive(false);
            return;
        }

        mCanvas.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        if (mAnimator.GetFloat("Speed") > 0.1f && _RunClip.Count() > 0)
        {
            mWalkTime += Time.deltaTime;
            if (mWalkTime >= mMaxWalkTime)
            {
                float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
                if(_RunClip.Count() > 0)
                    AudioManager.PlaySfx(_RunClip.ElementAt(UnityEngine.Random.Range(0, _RunClip.Count())).Clip, Mathf.Clamp01((distance - maxDistance) / (minDistance - maxDistance)));
                mWalkTime = 0.0f;
            }
        }
    }

    public void EnemySpawn(int id)
    {
        if (id == this.id)
        {
            mCollider.enabled = false;
            mRigidbody.isKinematic = true;
            mModel.SetActive(false);
            mCanvas.transform.Find("Tutorial").gameObject.SetActive(false);
            StartCoroutine(WaitForSpawn());
        }
    }

    public void Win(int id, Action onAction = null)
    {
        if(id == this.id)
        {
            for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
                Destroy(mEnemySpawnGroup[i], 1.0f);
            mEnemySpawnGroup.Clear();
        }
        onBattle = false;
        mModel.SetActive(true);
        ChangeBehavior("Idle");
        onAction?.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator WaitForSpawn()
    { 
        for (int i = 0; i < mEnemySpawnGroup.Count; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            mEnemySpawnGroup[i].GetComponent<Unit>().EnableUnit(i);
        }
        
        mExclamation.SetActive(false);
        mParticles.SetActive(false);
        mCanvas.SetActive(false);
        onBattle = true;
    }    

    private void OnDestroy()
    {
        GameManager.Instance.onBattle -= EnemySpawn;
        GameManager.Instance.onEnemyWin -= Win;
    }
}
