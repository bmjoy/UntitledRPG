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

        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.IsDied)
            {
                mInitialized = true;
                PlayerController.Instance.ResetPlayerUnit();
                PlayerController.Instance.OnBattleEnd();
                mObject = PlayerController.Instance.gameObject;
                PlayerController.Instance.transform.GetComponent<CharacterController>().enabled = false;
                PlayerController.Instance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                PlayerController.Instance.transform.GetComponent<CharacterController>().enabled = true;
                mObject.GetComponent<PlayerController>().mModel.GetComponent<Billboard>().Initialize();
            }
            else
            {
                Debug.Log(transform.position.x + " " + transform.position.z);
                PlayerController.Instance.transform.GetComponent<CharacterController>().enabled = false;
                PlayerController.Instance.transform.localPosition = transform.localPosition;
                PlayerController.Instance.transform.GetComponent<CharacterController>().enabled = true;
                PlayerController.Instance.GetComponent<PlayerController>().mModel.GetComponent<Billboard>().Initialize();
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
        mObject = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Player"), transform.position, Quaternion.identity);
        GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/Units/Allys/" + mName), transform.position, Quaternion.identity);
        unit.transform.SetParent(mObject.transform);
        mObject.GetComponent<PlayerController>().mHeroes.Add(unit);
        unit.SetActive(false);
        mObject.GetComponent<PlayerController>().ResetPlayerUnit();
        if (GameManager.Instance.mCamera)
            GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
    GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = mObject.transform;
        return mObject;
    }
}
