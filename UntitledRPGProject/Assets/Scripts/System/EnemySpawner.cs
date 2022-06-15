using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : Spawner
{
    [SerializeField]
    public List<EnemyUnit> mEnemyList = new List<EnemyUnit>();

    [SerializeField]
    private float mRadius = 5.0f;
    [SerializeField]
    private float mAngle = 30.0f;
    [SerializeField]
    private float mSpeed = 3.0f;
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

            GameObject newModel = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Enemys/" + mEnemyList[LeaderCount].ToString()), newEnemyProwler.transform.position, Quaternion.identity, newEnemyProwler.transform);
            newEnemyProwler.tag = "EnemyProwler";
            newEnemyProwler.layer = 6;
            newEnemyProwler.AddComponent<EnemyProwler>().Setup(mRadius, mAngle, mSpeed, ID, newModel.gameObject);

            for (int i = 0; i < mEnemyList.Count; i++)
            {
                if (mEnemyList[i] == EnemyUnit.None)
                    continue;
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Enemys/" + mEnemyList[i].ToString() + "_Unit"), transform.position, Quaternion.identity, newEnemyProwler.transform);
                newEnemyProwler.GetComponent<EnemyProwler>().mEnemySpawnGroup.Add(obj);
                obj.SetActive(false);
            }

            newEnemyProwler.GetComponent<EnemyProwler>()._RunClip = newEnemyProwler.GetComponent<EnemyProwler>().mEnemySpawnGroup[0].GetComponent<Unit>().mSetting.Clips.FindAll(
                delegate (SoundClip s)
                {
                    return s.Type == SoundClip.SoundType.Run;
                });

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
