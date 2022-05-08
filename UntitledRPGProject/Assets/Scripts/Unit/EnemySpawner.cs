using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    enum EnemyUnit
    {
        None,
        Ghoul,
        Spitter
    }
    private int ID = 0;
    private bool isInitialized = false;

    [SerializeField]
    private float mDelaySpawnTime = 1.0f;

    [SerializeField]
    private List<EnemyUnit> mEnemyList = new List<EnemyUnit>();

    [SerializeField]
    private float mRadius = 100.0f;
    [SerializeField]
    private float mAngle = 60.0f;

    public void StartSpawn()
    {
        if (isInitialized)
            return;
        ID = GameManager.s_ID++;
        StartCoroutine(EnemySpawn());
    }

    private GameObject CreateNewEnemyProwler()
    {
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
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/"+ mEnemyList[i].ToString() + "_Unit"),transform.position, Quaternion.identity);
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

    private IEnumerator EnemySpawn()
    {
        yield return new WaitForSeconds(mDelaySpawnTime);
        GameObject newEnemy = CreateNewEnemyProwler();
        if(newEnemy == null)
        {
            Debug.Log("Failed to create");
            isInitialized = false;
        }
        else
            isInitialized = true;
    }

    public void ResetSpawn()
    {
        isInitialized = false;
        StartSpawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1.0f,1.0f,1.0f));
    }
}
