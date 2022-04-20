using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    private int ID = 0;
    private bool isInitialized = false;

    [SerializeField]
    private float mDelaySpawnTime = 1.0f;

    [SerializeField]
    private List<GameObject> mEnemyList = new List<GameObject>();
    public GameObject mModelPrefab;

    [SerializeField]
    private float mRadius = 100.0f;
    [SerializeField]
    private float mAngle = 60.0f;
    public void StartSpawn()
    {
        ID = GameManager.s_ID++;
        StartCoroutine(EnemySpawn());
    }

    private GameObject CreateNewEnemyProwler()
    {
        if (mEnemyList.Count > 0)
        {
            GameObject newEnemyProwler = new GameObject("Enemy" + " " + ID);
            newEnemyProwler.transform.position = new Vector3(transform.position.x, 
                transform.position.y + 2.0f, 
                transform.position.z);
            GameObject newModel = Instantiate(mModelPrefab, newEnemyProwler.transform.position, Quaternion.identity);
            newModel.transform.parent = newEnemyProwler.transform;
            newEnemyProwler.tag = "EnemyProwler";
            newEnemyProwler.layer = 6;
            newEnemyProwler.AddComponent<BoxCollider>();
            newEnemyProwler.AddComponent<NavMeshAgent>();
            newEnemyProwler.AddComponent<EnemyProwler>();
            newEnemyProwler.GetComponent<EnemyProwler>().Setup(mRadius, mAngle, ID, newModel.gameObject);

            for (int i = 0; i < mEnemyList.Count; i++)
            {
                GameObject obj = Instantiate(mEnemyList[i].gameObject,transform.position, Quaternion.identity);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(1.0f,1.0f,1.0f));

    }
}
