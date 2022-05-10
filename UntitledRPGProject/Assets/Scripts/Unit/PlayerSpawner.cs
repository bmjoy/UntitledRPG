using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    GameObject mPlayer;
    public string mUnitElement = "Jimmy";
    public void Respawn()
    {
        if(mPlayer)
        {
            Debug.Log("Destory" + mPlayer.name);
            GameManager.Instance.mPlayer = null;
            Destroy(mPlayer);
            mPlayer = null;
        }
        
        mPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), transform.position,Quaternion.identity);

        GameObject unit = Instantiate(Resources.Load<GameObject>("Prefabs/" + mUnitElement), transform.position, Quaternion.identity);
        unit.transform.SetParent(mPlayer.transform);
        mPlayer.GetComponent<PlayerController>().mHeroes.Add(unit);
        unit.SetActive(false);

        GameManager.Instance.mPlayer = mPlayer.GetComponent<PlayerController>();
        GameManager.Instance.mPlayer.ResetPlayerUnit();
        GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow =
            GameManager.Instance.mCamera.transform.Find("GameWorldCamera").GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = mPlayer.transform;
    }

    public void Spawn()
    {
        Instantiate(mPlayer, transform.position, Quaternion.identity);
    }
}
