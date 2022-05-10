using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float mSpeed = 5.0f;
    [SerializeField]
    private float mGroundDistance = 2.0f;
    private float mTrunSmoothVelocity = 0.0f;
    private bool isGrounded = true;

    private ControlState mState = new IdleState();

    public GameObject mModel;
    public List<GameObject> mHeroes = new List<GameObject>();

    private Vector3 mVelocity = Vector3.zero;
    private BoxCollider mCollider;
    private CharacterController mCharacterController;
    private SpriteRenderer mSpriteRenderer;
    private GameObject mGroundCheck;
    private Transform mCamera;

    private bool isLeft = false;
    public bool onBattle = false;
    public int mGold = 0;
    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        GameObject groundCheck = new GameObject("GroundCheck");
        mModel = transform.Find("Model").gameObject;
        groundCheck.transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
        groundCheck.transform.parent = transform;
        mGroundCheck = groundCheck;
        GameManager.Instance.mPlayer = this;
        mCharacterController = GetComponent<CharacterController>();
        GameManager.Instance.onPlayerBattleStart += OnBattleStart;
        GameManager.Instance.onPlayerBattleEnd += OnBattleEnd;
        mAnimator = transform.GetComponentInChildren<Animator>();
        mSpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        mCollider = GetComponent<BoxCollider>();

        for (int i = 0; i < mHeroes.Count; ++i)
        {
            mHeroes[i].GetComponent<Player>().Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mModel.activeInHierarchy == false)
            return;

        if (mCamera == null)
            mCamera = GameObject.FindGameObjectWithTag("Camera").transform.Find("GameWorldCamera");

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
            }
        }

        if (isGrounded && mVelocity.y <= 0.0f)
            mVelocity.y = -2.0f;

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
    }

    private GameObject[] fields;

    public void ResetPlayerUnit()
    {
        mHeroes.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "PlayerUnit")
                mHeroes.Add(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < mHeroes.Count; ++i)
        {
            mHeroes[i].GetComponent<Unit>().ResetUnit();
            mHeroes[i].SetActive(false);
        }
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
            mHeroes[i].transform.position = BattleManager.Instance.playerCenter;
            mHeroes[i].GetComponent<Unit>().mField = BattleManager.playerFieldParent.GetChild(i).gameObject;
            mHeroes[i].GetComponent<Unit>().mAiBuild.actionEvent = ActionEvent.IntroWalk;
            mHeroes[i].gameObject.SetActive(true);
        }

        onBattle = true;
    }

    public void OnBattleEnd()
    {
        mCharacterController.enabled = mCollider.enabled = true;
        mState = new IdleState();
        onBattle = false;
        mModel.SetActive(true);
        for (int i = 0; i < mHeroes.Count; ++i)
        {
            mHeroes[i].transform.position = transform.position;
            mHeroes[i].GetComponent<Unit>().DisableUI();
            mHeroes[i].gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (onBattle)
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
