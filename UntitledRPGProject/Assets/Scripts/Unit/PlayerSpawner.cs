using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : Spawner
{
    public string mName;
    public override void Spawn()
    {
        if (mInitialized)
            return;

        if (GameManager.Instance.mPlayer != null)
        {
            if (GameManager.Instance.mPlayer.IsDied)
            {
                mInitialized = true;
                GameManager.Instance.mPlayer.ResetPlayerUnit();
                GameManager.Instance.mPlayer.OnBattleEnd();
                mObject = GameManager.Instance.mPlayer.gameObject;
                GameManager.Instance.mPlayer.transform.GetComponent<CharacterController>().enabled = false;
                GameManager.Instance.mPlayer.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                GameManager.Instance.mPlayer.transform.GetComponent<CharacterController>().enabled = true;
                mObject.GetComponent<PlayerController>().mModel.GetComponent<Billboard>().Initialize();
            }
            else
            {
                Debug.Log(transform.position.x + " " + transform.position.z);
                GameManager.Instance.mPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                GameManager.Instance.mPlayer.transform.GetComponent<CharacterController>().enabled = false;
                GameManager.Instance.mPlayer.transform.localPosition = transform.localPosition;
                GameManager.Instance.mPlayer.transform.GetComponent<CharacterController>().enabled = true;
                GameManager.Instance.mPlayer.GetComponent<PlayerController>().mModel.GetComponent<Billboard>().Initialize();
            }

        }
        else
        {
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
    protected override GameObject CreateNewObject()
    {
        if (mObject)
        {
            Debug.Log("Destory" + mObject.name);
            GameManager.Instance.mPlayer = null;
            Destroy(mObject);
            mObject = null;
        }

        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), transform.position, Quaternion.identity);
        mObject.GetComponent<InteractSystem>().Initialize();
        GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/" + mName), transform.position, Quaternion.identity);
        unit.transform.SetParent(mObject.transform);
        mObject.GetComponent<PlayerController>().mHeroes.Add(unit);
        unit.SetActive(false);
        Debug.Log(mObject.GetComponent<PlayerController>().mHeroes.Count);
        mObject.GetComponent<PlayerController>().ResetPlayerUnit();
        if (GameManager.Instance.mCamera)
        {
            GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
    GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = mObject.transform;
        }
        DontDestroyOnLoad(mObject);
        return mObject;
    }
}
