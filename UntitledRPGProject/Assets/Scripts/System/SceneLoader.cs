using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject mLoadingScreen;
    public int _sceneIndex = 1;
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
        DontDestroyOnLoad(gameObject);

        Instantiate(Resources.Load<GameObject>("Prefabs/Managers/GameManager"),Vector3.zero,Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/Managers/LevelManager"), Vector3.zero, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/Managers/BattleManager"), Vector3.zero, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/Managers/UIManager"), Vector3.zero, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/Managers/AudioManager"), Vector3.zero, Quaternion.identity);
        Instantiate(Resources.Load<GameObject>("Prefabs/GameCamera"), Vector3.zero, Quaternion.identity);

        LevelManager.Instance.mCurrentLevel = _sceneIndex;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.Instance.mCurrentLevel++;
            _sceneIndex = LevelManager.Instance.mCurrentLevel;
            StartGame();
        }
    }

    public void StartGame()
    {
        mLoadingScreen.GetComponent<LoadingScreen>().mProgressBar.value = 0;
        StartCoroutine(StartLoad());
    }

    IEnumerator StartLoad()
    {
        mLoadingScreen.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(1, 2));
        AsyncOperation operation = null;
        operation = SceneManager.LoadSceneAsync(_sceneIndex);

        while(!operation.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(0, 3));
        mLoadingScreen.SetActive(false);
        if(LevelManager.Instance)
            LevelManager.Instance.isNext = false;
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
