using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager mInstance;
    public static GameManager Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }
    public GameState mGameState;
    public GameObject mPlayer;
    public GameObject mEnemyProwler;

    public Queue<GameObject> mOrders = new Queue<GameObject>();

    [SerializeField]
    private GameObject mField;
    public GameObject mCurrentField;

    private bool isBattle = false;

    private void Start()
    {
        mGameState = gameObject.GetComponent<GameState>();
        mCurrentField = GameObject.Instantiate(Instance.mField, Vector3.zero, Quaternion.identity);
        mCurrentField.SetActive(false);
    }
    private void Update()
    {
        UpdateGameState();
    }
    private void UpdateGameState()
    {
        switch (mGameState)
        {
            case GameState.MainMenu:
                break;
            case GameState.GamePlay:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Time.timeScale = 0.0f;
                        mGameState = GameState.GamePause;
                    }
                }
                break;
            case GameState.GamePause:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        Time.timeScale = 1.0f;
                        mGameState = GameState.GamePlay;
                    }
                }
                break;
            case GameState.Battle:
                {
                    SetBattleField();
                    CameraSwitcher.SwitchCamera();
                }
                break;
            case GameState.Victory:
                {
                    Instance.mCurrentField.SetActive(false);
                }
                break;
            case GameState.GameOver:
                {
                    Instance.mCurrentField.SetActive(false);
                }
                break;
            default:
                break;
        }

    }

    private IEnumerator UpdateBettle()
    {
        yield return new WaitForSeconds(2.0f);
    }

    public void SetBattleField()
    {
        Vector3 mOffset = mPlayer.transform.localPosition;
        Vector3 mTargetOffset = mEnemyProwler.transform.localPosition;

        Vector3 point = mOffset + 0.5f * (mTargetOffset - mOffset);

        mCurrentField.transform.position = new Vector3(mOffset.x, mOffset.y, mOffset.z + 5.0f);
        CameraSwitcher.UpdateCamera(mCurrentField.transform);
        mCurrentField.SetActive(true);
    }

    public event Action onPlayerBattleStart;
    public event Action onPlayerBattleEnd;
    public event Action<int> onBattle;
    public void OnBattleStart(int id)
    {
        onPlayerBattleStart?.Invoke();
        onBattle?.Invoke(id);
    }

    public event Action<int> onEnemyDeath;
    public void OnEnemyDeath(int id)
    {
        onEnemyDeath?.Invoke(id);
    }

    public void OnBattleEnd()
    {
        onPlayerBattleEnd?.Invoke();
    }
}
