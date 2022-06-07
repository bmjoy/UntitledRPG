using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        GameManager.Instance.onGameOverToReset += RestartGame;
    }

    public int mCurrentLevel = 0;
    public void RespawnEntities()
    {
        var obj = GameObject.FindObjectOfType<UnitSpawnManager>();
        obj.ResetSpawnAll();
    }

    public void GoNextLevel()
    {
        mCurrentLevel++;
        SceneLoader.Instance._sceneIndex += 1;
        SceneLoader.Instance.mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        SceneLoader.Instance.StartGame();
    }

    public void GoBackLevel()
    {
        mCurrentLevel--;
        SceneLoader.Instance._sceneIndex -= 1;
        SceneLoader.Instance.mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        SceneLoader.Instance.StartGame();
    }

    public void RestartGame()
    {
        SceneLoader.Instance._sceneIndex = mCurrentLevel = 2;
        SceneLoader.Instance.mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        SceneLoader.Instance.StartGame();
        mCurrentLevel = 2;
    }
}
