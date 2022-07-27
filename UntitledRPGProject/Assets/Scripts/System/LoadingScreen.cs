using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public int _sceneToLoad = 0;
    public Slider mProgressBar;
    AsyncOperation mAsyncOperation;

    void Start()
    {
        mProgressBar.value = 0;
        mAsyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad);
    }

    void Update()
    {
        mProgressBar.value = Mathf.Clamp01(mAsyncOperation.progress / 0.9f);
    }
}
