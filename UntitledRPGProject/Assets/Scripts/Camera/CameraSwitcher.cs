using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraSwitcher : MonoBehaviour
{
    private static CameraSwitcher mInstance;
    public static CameraSwitcher Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        mAnimator = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    private CinemachineVirtualCamera mBattleWorldCam;


    public CinemachineStateDrivenCamera mCamera;
    private void Start()
    {
        mCamera = this.GetComponent<CinemachineStateDrivenCamera>();
    }
    private Animator mAnimator;
    private bool isGameWorld = true;

    public static void SwitchCamera()
    {
        if (Instance.isGameWorld)
        {
            Instance.mAnimator.Play("BattleWorld");
        }
        else
        {
            Instance.mAnimator.Play("GameWorld");
        }
        Instance.isGameWorld = !Instance.isGameWorld;
    }

    public static void UpdateCamera(Transform transform)
    {
        Instance.mBattleWorldCam.m_Follow = Instance.mBattleWorldCam.m_LookAt = transform;
    }
}
