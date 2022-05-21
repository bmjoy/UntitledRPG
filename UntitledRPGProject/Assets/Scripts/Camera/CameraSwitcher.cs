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

    [SerializeField] private float mShakeAmount; 
    [SerializeField] private float mFrequency;

    [SerializeField] private float mMaximumZoomFOV = 10.0f;
    private float mCurrentFOV = 0.0f;

    public CinemachineStateDrivenCamera mCamera;
    public VolumeProfile mPostProcessing;

    public Bloom mBloom;
    private void Start()
    {
        mCamera = this.GetComponent<CinemachineStateDrivenCamera>();
        mCurrentFOV = mBattleWorldCam.m_Lens.FieldOfView;
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

    bool _isCameraUsing = false;
    public IEnumerator ShakeCamera(float duration)
    {
        if (_isCameraUsing)
            yield return null;
        else
        {
            _isCameraUsing = true;
            Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = mShakeAmount;
            Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = mFrequency;
            yield return new WaitForSeconds(duration);
            Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain =
                Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0.0f;
            _isCameraUsing = false;
        }
    }

    public IEnumerator ZoomCamera(float duration, Vector3 pos)
    {
        if (_isCameraUsing)
            yield return null;
        else
        {
            _isCameraUsing = true;

            GameObject fov = new GameObject("FOV");
            fov.transform.position = pos;

            mBattleWorldCam.m_Follow = fov.transform;
            mBattleWorldCam.m_LookAt = fov.transform;
            while (mBattleWorldCam.m_Lens.FieldOfView - 0.00001f > mMaximumZoomFOV)
            {
                mBattleWorldCam.m_Lens.FieldOfView = Mathf.Lerp(mBattleWorldCam.m_Lens.FieldOfView, mMaximumZoomFOV, Time.deltaTime * 15.0f);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(duration);
            mBattleWorldCam.m_Follow = BattleManager.Instance.mCurrentField.transform;
            mBattleWorldCam.m_LookAt = BattleManager.Instance.mCurrentField.transform;
            while (mBattleWorldCam.m_Lens.FieldOfView <= mCurrentFOV - 0.00001f)
            {
                mBattleWorldCam.m_Lens.FieldOfView = Mathf.Lerp(mBattleWorldCam.m_Lens.FieldOfView, mCurrentFOV, Time.deltaTime * 15.0f);
                yield return new WaitForEndOfFrame();
            }
            mBattleWorldCam.m_Lens.FieldOfView = mCurrentFOV;
            Destroy(fov);
            _isCameraUsing = false;
        }
    }
}
