using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    GameObject loader;
    public Canvas mCanvas;
    public Button button;

    void Start()
    {
        loader = GameObject.Find("GameLoader");
        button.onClick.AddListener(()=>loader.GetComponent<SceneLoader>().StartGame());
        button.onClick.AddListener(()=>mCanvas.sortingOrder = -1);
    }
}
