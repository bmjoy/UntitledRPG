using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private static PlayerController mInstance;
    public static PlayerController Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
            Destroy(gameObject);
        else
            mInstance = this;
        DontDestroyOnLoad(gameObject);
    }


    [SerializeField]
    private float mSpeed = 5.0f;
    [SerializeField]
    private float mGroundDistance = 2.0f;
    private float mTrunSmoothVelocity = 0.0f;
    private bool isGrounded = true;
    public bool IsDied = false;

    public ControlState mState = new IdleState();

    public GameObject mModel;
    public List<GameObject> mHeroes = new List<GameObject>();

    private Vector3 mVelocity = Vector3.zero;
    private BoxCollider mCollider;
    private CharacterController mCharacterController;
    private SpriteRenderer mSpriteRenderer;
    private GameObject mGroundCheck;
    private Transform mCamera;
    private InteractSystem mInteractSystem;

    public Inventory mInventory;
    public GameObject mCanvas;
    public GameObject mBag;

    private bool isLeft = false;
    public bool onBattle = false;
    public int mGold = 0;
    public int mSoul = 0;
    private Animator mAnimator;

    [SerializeField]
    private List<AudioClip> _RunClips = new List<AudioClip>();
    private float mWalkTime = 0.0f;
    [SerializeField]
    private float mEveryWalkTime = 0.3f;
    public bool Interaction { get 
        {
            if (mInteractSystem == null)
                return false;
            return mInteractSystem.IsInteracting; 
        } 
    }
    void Start()
    {
        if (Instance.mInventory == null)
            Instance.mInventory = new Inventory();
        if (transform.Find("GroundCheck") == null)
        {
            GameObject groundCheck = new GameObject("GroundCheck");
            mModel = transform.Find("Model").gameObject;
            groundCheck.transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
            groundCheck.transform.parent = transform;
            mGroundCheck = groundCheck;
        }

        if(mBag == null)
        {
            GameObject myBag = new GameObject("Bag");
            myBag.transform.SetParent(gameObject.transform);
            mBag = myBag;
        }

        GameManager.Instance.onPlayerBattleStart += OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd += OnBattleEnd;
        mCharacterController = GetComponent<CharacterController>();
        mAnimator = transform.GetComponentInChildren<Animator>();
        mCanvas = transform.Find("Canvas").gameObject;
        mCanvas.SetActive(false);
        mSpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        mCollider = GetComponent<BoxCollider>();
        mInteractSystem = GetComponent<InteractSystem>();

        if (mCamera == null)
            mCamera = CameraSwitcher.Instance.transform.Find("GameWorldCamera");
    }

    void Update()
    {
        if (!IsDied && PlayerController.Instance.mHeroes.TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
        {
            if(!IsDied)
                GameManager.Instance.mGameState = GameState.GameOver;
            IsDied = true;
            return;
        }
        if (mModel.activeInHierarchy == false || GameManager.Instance.IsCinematicEvent || Interaction)
            return;
        mState = mState.Handle();
        StateControl();
        isGrounded = Physics.CheckSphere(mGroundCheck.transform.position, mGroundDistance, LayerMask.GetMask("Ground"));
        if(mState.ToString() != "BattleState")
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(x, 0.0f, z).normalized;
            if (direction.magnitude > 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref mTrunSmoothVelocity, 0.15f);
                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
                Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                mCharacterController.Move(moveDirection.normalized * mSpeed * Time.deltaTime);

                mWalkTime += Time.deltaTime;
                if(mWalkTime >= mEveryWalkTime)
                {
                    if(_RunClips.Count > 0)
                        AudioManager.PlaySfx(_RunClips[UnityEngine.Random.Range(0, _RunClips.Count - 1)], 0.6f);
                    mWalkTime = 0.0f;
                }
            }
        }

        if (isGrounded && mVelocity.y <= 0.0f) mVelocity.y = -2.0f;
        mVelocity.y += -9.8f * Time.deltaTime;
        mCharacterController.Move(mVelocity * Time.deltaTime);
    }

    private void StateControl()
    {
        switch(mState.ToString())
        {
            case "IdleState":
                mAnimator.SetFloat("Speed", 0.0f);
                break;
            case "RunState":
                {
                    if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isLeft)
                    {
                        isLeft = true;
                        mSpriteRenderer.flipX = false;
                    }
                    if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && isLeft)
                    {
                        isLeft = false;
                        mSpriteRenderer.flipX = true;
                    }
                    mAnimator.SetFloat("Speed", mSpeed);
                }
                break;
        }
        if(PlayerController.Instance.mHeroes.TrueForAll(t => t.GetComponent<Unit>().mConditions.isDied))
        {
            mAnimator.SetBool("Death", true);
        }
    }

    public void ResetPlayerUnit()
    {
        mHeroes.Clear();
        GameObject go = new GameObject("J");

        bool finish = false;
        while(!finish)
        {
            bool exist = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).tag == "PlayerCompanion")
                {
                    transform.GetChild(i).transform.SetParent(go.transform);
                    exist = true;
                }
            }
            if (finish == exist) break;
        }
        mInventory?.AllDelete();

        Destroy(go);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "PlayerUnit")
                mHeroes.Add(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < mHeroes.Count; ++i)
        {
            mHeroes[i].GetComponent<Unit>().ResetUnit();
            mHeroes[i].GetComponent<Billboard>().Initialize();
            mHeroes[i].SetActive(false);
        }
        mAnimator?.SetBool("Death", false);
    }

    public void OnBattleStart()
    {
        mState = new BattleState();
        mCharacterController.enabled = mCollider.enabled = false;
        mModel.SetActive(false);
        StartCoroutine(WaitForSpawn());
    }

    private IEnumerator WaitForSpawn()
    {
        for (int i = 0; i < mHeroes.Count; ++i)
        {
            yield return new WaitForSeconds(0.1f);
            mHeroes[i].GetComponent<Unit>().EnableUnit(i);
        }

        onBattle = true;
    }

    public void OnBattleEnd()
    {
        IsDied = onBattle = false;
        mCharacterController.enabled = mCollider.enabled = true;
        mState = new IdleState();
        mModel.SetActive(true);
        for (int i = 0; i < mHeroes.Count; ++i)
            mHeroes[i].GetComponent<Unit>().DisableUnit(transform.position);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (onBattle || LevelManager.Instance.isNext)
            return;

        var unit = other.GetComponent<EnemyProwler>();

        if (unit != null && !unit.onBattle)
        {
            GameManager.Instance.mEnemyProwler = unit;
            GameManager.Instance.OnBattleStart(unit.id);
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.onPlayerBattleStart -= OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd -= OnBattleEnd;
    }

    private void OnDestroy()
    {
        GameManager.Instance.onPlayerBattleStart -= OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd -= OnBattleEnd;
    }
}
