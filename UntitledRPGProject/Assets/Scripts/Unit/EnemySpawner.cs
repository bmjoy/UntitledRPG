using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : Spawner
{
    enum EnemyUnit
    {
        None,
        Ghoul,
        Spitter
    }
    [SerializeField]
    private List<EnemyUnit> mEnemyList = new List<EnemyUnit>();

    [SerializeField]
    private float mRadius = 100.0f;
    [SerializeField]
    private float mAngle = 60.0f;

    protected override GameObject CreateNewObject()
    {
        if (mEnemyList.Count == 1 && mEnemyList[0] == EnemyUnit.None)
            return null;

        if (mEnemyList.Count > 0)
        {
            GameObject newEnemyProwler = new GameObject("Enemy" + " " + ID);
            newEnemyProwler.transform.position = new Vector3(transform.position.x,
                transform.position.y + 2.5f,
                transform.position.z);

            int LeaderCount = 0;
            for (int i = 0; i < mEnemyList.Count; ++i)
            {
                if (mEnemyList[i] == EnemyUnit.None)
                    LeaderCount++;
                else
                    break;
            }

            GameObject newModel = Instantiate(Resources.Load<GameObject>("Prefabs/" + mEnemyList[LeaderCount].ToString()), newEnemyProwler.transform.position, Quaternion.identity);
            newModel.transform.parent = newEnemyProwler.transform;
            newEnemyProwler.tag = "EnemyProwler";
            newEnemyProwler.layer = 6;
            newEnemyProwler.AddComponent<EnemyProwler>().Setup(mRadius, mAngle, ID, newModel.gameObject);

            for (int i = 0; i < mEnemyList.Count; i++)
            {
                if (mEnemyList[i] == EnemyUnit.None)
                    continue;
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/" + mEnemyList[i].ToString() + "_Unit"), transform.position, Quaternion.identity);
                newEnemyProwler.GetComponent<EnemyProwler>().mEnemySpawnGroup.Add(obj);
                obj.transform.parent = newEnemyProwler.transform;
                obj.SetActive(false);
            }

            newEnemyProwler.GetComponent<EnemyProwler>().Initialize();
            return newEnemyProwler;
        }
        else
            return null;
    }

    public override void Spawn()
    {
        if (mInitialized)
            return;
        ID = GameManager.s_ID++;
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
