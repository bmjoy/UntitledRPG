using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager mInstance;
    public static UIManager Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Canvas mCanvas;
    public GameObject mBattleUI;

    // Start is called before the first frame update
    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        DisplayBattleInterface(false);
    }

    public void DisplayBattleInterface(bool display)
    {
        mBattleUI.SetActive(display);
    }
}
