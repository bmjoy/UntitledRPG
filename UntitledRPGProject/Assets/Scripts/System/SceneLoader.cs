using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class SceneLoader : MonoBehaviour
{
    public GameObject mLoadingScreen;
    public int _sceneIndex = 0;
    public CanvasGroup mCanvasGroup;

    private static SceneLoader mInstance;
    public static SceneLoader Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Managers/GameManager"),Vector3.zero,Quaternion.identity);
        Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Managers/LevelManager"), Vector3.zero, Quaternion.identity);
        Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Managers/BattleManager"), Vector3.zero, Quaternion.identity);
        Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Managers/UIManager"), Vector3.zero, Quaternion.identity);
        Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/Managers/AudioManager"), Vector3.zero, Quaternion.identity);
        Instantiate(ResourceManager.GetResource<GameObject>("Prefabs/GameCamera"), Vector3.zero, Quaternion.identity);
        LevelManager.Instance.mCurrentLevel = _sceneIndex;
        StartGame();
    }

    public void StartGame()
    {
        StopAllCoroutines();
        LevelManager.Instance.mCurrentLevel = _sceneIndex;
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad()
    {
        mLoadingScreen.SetActive(true);
        mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0.0f;
        yield return StartCoroutine(FadeLoadingScreen(1, 2));
        AsyncOperation operation = null;
        operation = SceneManager.LoadSceneAsync(_sceneIndex);

        while(!operation.isDone)
        {
            yield return null;
        }
        if (LevelManager.Instance)
        {
            LevelManager.Instance.isNext = false;
            LevelManager.Instance.InitializeUICamera();
        }
        yield return StartCoroutine(FadeLoadingScreen(0, 3));
        mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0.0f;
        mLoadingScreen.SetActive(false);
    }

    IEnumerator FadeLoadingScreen(float val, float duration)
    {
        float startValue = mCanvasGroup.alpha;
        float time = 0.0f;
        while(time < duration)
        {
            mCanvasGroup.alpha = Mathf.Lerp(startValue, val, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        mCanvasGroup.alpha = val;
    }
}
