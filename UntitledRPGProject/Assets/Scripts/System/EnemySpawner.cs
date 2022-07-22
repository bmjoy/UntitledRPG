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
        if (mEnemyList.Count == 0)
            return null;
        GameObject group = (GameObject.Find("Enemies")) ? GameObject.Find("Enemies").gameObject : new GameObject("Enemies");
        GameObject newEnemyProwler = new GameObject("Enemy" + " " + ID);
        newEnemyProwler.transform.SetParent(group.transform);
        newEnemyProwler.transform.position = new Vector3(transform.position.x,
            transform.position.y + 1.0f,
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
        newEnemyProwler.layer = LayerMask.NameToLayer("EnemyProwler");
        newEnemyProwler.AddComponent<EnemyProwler>().Setup(mRadius, mAngle, mSpeed, ID, newModel.gameObject);
        EnemyProwler enemyProwler = newEnemyProwler.GetComponent<EnemyProwler>();
        for (int i = 0; i < mEnemyList.Count; i++)
        {
            if (mEnemyList[i] == EnemyUnit.None)
                continue;
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Enemys/" + mEnemyList[i].ToString() + "_Unit"), transform.position, Quaternion.identity, newEnemyProwler.transform);
            enemyProwler.mEnemySpawnGroup.Add(obj);
            obj.SetActive(false);
        }

        enemyProwler._RunClip = enemyProwler.mEnemySpawnGroup[0].GetComponent<Unit>().mSetting.Clips.FindAll(
            delegate (SoundClip s)
            {
                return s.Type == SoundClip.SoundType.Run;
            });

        enemyProwler.Initialize(this);
        return newEnemyProwler;
    }

    public override void Spawn(bool isDungeon = false)
    {
        if (mInitialized)
            return;
        ID = GameManager.s_ID++;
        if (isDungeon == false)
        {
            GameObject icon = Instantiate(Resources.Load<GameObject>((mEnemyList.Contains(EnemyUnit.Temple_Guardian) || mEnemyList.Contains(EnemyUnit.The_Bloody_King)) ? "Prefabs/UI/Icon/BossEnemyIcon" : "Prefabs/UI/Icon/NormalEnemyIcon"), transform.position, Quaternion.identity);
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
