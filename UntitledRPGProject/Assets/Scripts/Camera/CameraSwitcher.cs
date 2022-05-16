using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public VolumeProfile mPostProcessing;

    public Bloom mBloom;
    private void Start()
    {
        mCamera = this.GetComponent<CinemachineStateDrivenCamera>();

        GameManager.Instance.onPlayerBattleStart += SwitchCamera;
        if (GameObject.Find("PostProcessing") == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/PostProcessing"));
            mPostProcessing = go.GetComponent<Volume>().profile;
            go.transform.SetParent(transform);
        }
        else
            mPostProcessing = GameObject.Find("PostProcessing").GetComponent<Volume>().profile;
        mPostProcessing.TryGet(out mBloom);
        
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
