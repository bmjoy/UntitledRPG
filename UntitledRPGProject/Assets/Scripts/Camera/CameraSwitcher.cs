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
    [SerializeField]
    private CinemachineVirtualCamera mGameWorldCam;

    private CinemachineCollider mGameWorldCollider;
    private CinemachineCollider mBattleWorldCollider;

    [SerializeField] private float mShakeAmount; 
    [SerializeField] private float mFrequency;

    [SerializeField] private float mMaximumZoomFOV = 10.0f;
    private float mCurrentFOV = 0.0f;
    private float mCurrentGameWorldFOV = 0.0f;

    public CinemachineStateDrivenCamera mCamera;
    public VolumeProfile mPostProcessing;

    public Bloom mBloom;
    public LiftGammaGain mLiftGammaGain;
    private void Start()
    {
        mCamera = this.GetComponent<CinemachineStateDrivenCamera>();
        mGameWorldCollider = mGameWorldCam.GetComponent<CinemachineCollider>();
        mBattleWorldCollider = mBattleWorldCam.GetComponent<CinemachineCollider>();
        mCurrentFOV = mBattleWorldCam.m_Lens.FieldOfView;
        mCurrentGameWorldFOV = mGameWorldCam.m_Lens.FieldOfView;
        GameManager.Instance.onPlayerBattleStart += SwitchCamera;
        if (GameObject.Find("PostProcessing") == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/PostProcessing"), transform);
            mPostProcessing = go.GetComponent<Volume>().profile;
        }
        else
            mPostProcessing = GameObject.Find("PostProcessing").GetComponent<Volume>().profile;
        mPostProcessing.TryGet(out mBloom);
        mPostProcessing.TryGet(out mLiftGammaGain);
        isInitialized = false;
        isCollided = false;
        mBattleWorldCam.m_Lens.FieldOfView = 10.0f;
    }
    private Animator mAnimator;
    private bool isGameWorld = true;
    public static bool isCollided = false;
    public static bool isInitialized = false;

    public static void CollideCheck()
    {
        if (Instance.mBattleWorldCollider.IsTargetObscured(Instance.mBattleWorldCam))
        {
            isCollided = true;
        }
        else
        {
            isCollided = false;
        }
        Instance.mBattleWorldCollider.m_AvoidObstacles = true;
    }
    float distance = 0.0f;

    private void FixedUpdate()
    {
        if (_isCameraUsing)
            return;
        if (isInitialized)
        {
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            distance = Vector3.Distance(camera.transform.position, BattleManager.Instance.mCurrentField.transform.position);
            if (isCollided)
            {
                Instance.mBattleWorldCam.m_Lens.FieldOfView = (distance > 25.0f) ? Mathf.Lerp(Instance.mBattleWorldCam.m_Lens.FieldOfView, 20.0f, Time.deltaTime * 3.0f) : Mathf.Lerp(Instance.mBattleWorldCam.m_Lens.FieldOfView, 45.0f, Time.deltaTime * 3.0f);
                Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, new Vector3(-90.0f, 25.0f, 0.0f), -Time.deltaTime * 3.0f);
            }
            else
            {
                Instance.mBattleWorldCam.m_Lens.FieldOfView = Mathf.Lerp(Instance.mBattleWorldCam.m_Lens.FieldOfView, 10.0f, Time.deltaTime * 3.0f);
                Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, new Vector3(90.0f, 25.0f, 0.0f), Time.deltaTime * 3.0f);
            }
        }

    }

    public static void SwitchCamera()
    {
        if (Instance.isGameWorld)
        {
            isInitialized = false;
            Instance.mBattleWorldCollider.m_AvoidObstacles = false;
            Instance.mAnimator.Play("BattleWorld");
        }
        else
        {
            Instance.mAnimator.Play("GameWorld");
            Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(90.0f, 25.0f, 0.0f);
            isInitialized = false;
            isCollided = false;
            Instance.mBattleWorldCam.m_Lens.FieldOfView = 10.0f;
            Instance.mBattleWorldCollider.m_AvoidObstacles = false;
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
    
    public static void StopShakeCamera()
    {
        Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain =
    Instance.mBattleWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0.0f;
        Instance._isCameraUsing = false;
    }
    
    public IEnumerator PlayShakeCameraGameWorld()
    {
        Instance.mGameWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = mShakeAmount;
        Instance.mGameWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = mFrequency;
        yield return null;
    }    
    public IEnumerator StopShakeCameraGameWorld()
    {
        Instance.mGameWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain =
            Instance.mGameWorldCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0.0f;
        yield return null;
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

            Transform temp_Follow = mBattleWorldCam.m_Follow;
            Transform temp_LookAt = mBattleWorldCam.m_LookAt;

            mBattleWorldCam.m_Follow = fov.transform;
            mBattleWorldCam.m_LookAt = fov.transform;
            while (mBattleWorldCam.m_Lens.FieldOfView - 0.00001f > mMaximumZoomFOV)
            {
                mBattleWorldCam.m_Lens.FieldOfView = Mathf.Lerp(mBattleWorldCam.m_Lens.FieldOfView, mMaximumZoomFOV, Time.deltaTime * 15.0f);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(duration);
            mBattleWorldCam.m_Follow = temp_Follow;
            mBattleWorldCam.m_LookAt = temp_LookAt;
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

    public IEnumerator ZoomCameraGameWorld(float duration, float maximum, Vector3 pos)
    {
        if (_isCameraUsing)
            yield return null;
        else
        {
            _isCameraUsing = true;

            GameObject fov = new GameObject("FOV");
            fov.transform.position = pos;

            Transform temp_Follow = mGameWorldCam.m_Follow;
            Transform temp_LookAt = mGameWorldCam.m_LookAt;

            mGameWorldCam.m_Follow = fov.transform;
            mGameWorldCam.m_LookAt = fov.transform;
            while (mGameWorldCam.m_Lens.FieldOfView - 0.00001f > maximum)
            {
                mGameWorldCam.m_Lens.FieldOfView = Mathf.Lerp(mGameWorldCam.m_Lens.FieldOfView, maximum, Time.deltaTime * 15.0f);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(duration);
            mGameWorldCam.m_Follow = temp_Follow;
            mGameWorldCam.m_LookAt = temp_LookAt;
            while (mGameWorldCam.m_Lens.FieldOfView <= mCurrentGameWorldFOV - 0.00001f)
            {
                mGameWorldCam.m_Lens.FieldOfView = Mathf.Lerp(mGameWorldCam.m_Lens.FieldOfView, mCurrentGameWorldFOV, Time.deltaTime * 15.0f);
                yield return new WaitForEndOfFrame();
            }
            mGameWorldCam.m_Lens.FieldOfView = mCurrentGameWorldFOV;
            Destroy(fov);
            _isCameraUsing = false;
        }
    }

    private void Reset()
    {
        isInitialized = false;
        isCollided = false;
        mBattleWorldCam.m_Lens.FieldOfView = 10.0f;
        mBattleWorldCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(90.0f, 25.0f, 0.0f);
    }
}
