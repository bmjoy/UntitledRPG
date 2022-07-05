using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{
    private static LevelManager mInstance;
    public static LevelManager Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool isNext = false;

    private void Start()
    {
        GameManager.Instance.onGameOverToReset += RestartGame;
    }

    public int mCurrentLevel = 0;

    public void RespawnEntities()
    {
        var obj = GameObject.FindObjectOfType<UnitSpawnManager>();
        obj.SpawnAll();
    }

    public IEnumerator GoNextLevel()
    {
        mCurrentLevel++;
        SceneLoader.Instance._sceneIndex += 1;
        SceneLoader.Instance.mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        SceneLoader.Instance.StartGame();
        yield return new WaitForSeconds(2.5f);
        RespawnEntities();
    }

    public void InitializeUICamera()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject;
        if (camera == null) return;

        var cameraData = camera.GetComponent<Camera>().GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Clear();
        cameraData.cameraStack.Add(UIManager.Instance.mUICamera);
    }

    public IEnumerator GoBackLevel()
    {
        mCurrentLevel--;
        SceneLoader.Instance._sceneIndex -= 1;
        SceneLoader.Instance.mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        SceneLoader.Instance.StartGame();
        yield return new WaitForSeconds(2.5f);
        RespawnEntities();
    }

    public void RestartGame()
    {
        SceneLoader.Instance._sceneIndex = mCurrentLevel = 2;
        SceneLoader.Instance.mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        SceneLoader.Instance.StartGame();
        mCurrentLevel = 2;
    }
}
