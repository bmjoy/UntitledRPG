using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : Spawner
{
    public EnemyUnit[] mEnemyList = new EnemyUnit[4] 
    { 
        EnemyUnit.None,
        EnemyUnit.None,
        EnemyUnit.None,
        EnemyUnit.None
    };

    [SerializeField]
    private float mRadius = 5.0f;
    [SerializeField]
    private float mAngle = 30.0f;
    [SerializeField]
    private float mSpeed = 3.0f;
    protected override GameObject CreateNewObject()
    {
        if (mEnemyList.Length == 0)
            return null;
        GameObject group = (GameObject.Find("Enemies")) ? GameObject.Find("Enemies").gameObject : new GameObject("Enemies");
        GameObject newEnemyProwler = new GameObject("Enemy" + " " + ID);
        newEnemyProwler.transform.SetParent(group.transform);
        newEnemyProwler.transform.position = new Vector3(transform.position.x,
            transform.position.y + 1.0f,
            transform.position.z);

        int LeaderCount = 0;
        bool found = false;
        int count = 0;
        for (int i = 0; i < mEnemyList.Length; ++i)
        {
            if (mEnemyList[i] == EnemyUnit.None)
                LeaderCount = (!found) ? LeaderCount++ : LeaderCount;
            else
            {
                count++;
                found = true;
            }
        }

        GameObject newModel = Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Units/Model"), newEnemyProwler.transform.position, Quaternion.identity, newEnemyProwler.transform);
        GameObject firstUnit = ResourceManager.GetResource<GameObject>("Prefabs/Units/Enemys/" + mEnemyList[LeaderCount].ToString() + "_Unit");
        newModel.GetComponent<SpriteRenderer>().sprite = firstUnit.GetComponent<SpriteRenderer>().sprite;
        newModel.GetComponent<BoxCollider>().size = firstUnit.GetComponent<BoxCollider>().size;
        newModel.GetComponent<Animator>().runtimeAnimatorController = firstUnit.GetComponent<Animator>().runtimeAnimatorController;
        newEnemyProwler.tag = "EnemyProwler";
        newEnemyProwler.layer = LayerMask.NameToLayer("EnemyProwler");
        newEnemyProwler.AddComponent<EnemyProwler>().Setup(mRadius, mAngle, mSpeed, ID, newModel.gameObject,mEnemyList,count,this);
        return newEnemyProwler;
    }

    public override void Spawn(bool isDungeon = false)
    {
        if (mInitialized)
            return;
        ID = GameManager.s_ID++;
        if (isDungeon == false)
        {
            GameObject icon = Instantiate(ResourceManager.GetResource<GameObject>((mEnemyList.Contains(EnemyUnit.Temple_Guardian) || mEnemyList.Contains(EnemyUnit.The_Bloody_King)) ? "Prefabs/UI/Icon/BossEnemyIcon" : "Prefabs/UI/Icon/NormalEnemyIcon"), transform.position, Quaternion.identity);
            if (icon != null)
                icon.transform.eulerAngles = new Vector3(90.0f, -90.0f, 0.0f);
        }
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f);
        mObject = CreateNewObject();
        if (mObject == null)
        {
            Debug.Log("Failed to create");
            mInitialized = false;
        }
        else
            mInitialized = true;
    }
}
