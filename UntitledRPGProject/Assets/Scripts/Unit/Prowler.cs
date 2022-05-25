using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prowler : MonoBehaviour
{
    public GameObject mModel;
    public NavMeshAgent mAgent;
    public Animator mAnimator;
    protected BoxCollider mCollider;
    protected ProwlerStateMachine mStateMachine;
    protected SpriteRenderer mSpriteRenderer;
    public int id = 0;
    public bool onBattle = false;

    public float mRadius = 5.0f;
    public float mAngle = 60.0f;
    public float mStandbyTime = 3.0f;

    public Vector3 mLastPos = Vector3.zero;
    public Vector3 mVelocity = Vector3.zero;
    public float mOriginalSpeed = 0.0f;

    protected virtual void Start()
    {
        mLastPos = transform.position;
    }

    public virtual void Setup(float rad, float ang, int _id, GameObject model)
    {
        mRadius = rad;
        mAngle = ang;
        id = _id;
        mModel = model;
    }

    public virtual void Initialize()
    {
        mCollider = gameObject.AddComponent<BoxCollider>();
        mAgent = gameObject.AddComponent<NavMeshAgent>();
        mCollider.isTrigger = true;
        mAgent.baseOffset = 2.0f;
        mAgent.speed = (mOriginalSpeed == 0.0f) ? 1.5f : mOriginalSpeed;
        mAnimator = mModel.GetComponent<Animator>();
        mSpriteRenderer = mModel.GetComponent<SpriteRenderer>();

        mAnimator.SetFloat("Speed", 0.0f);
        mOriginalSpeed = mAgent.speed;

        mStateMachine = gameObject.AddComponent<ProwlerStateMachine>();
        mStateMachine.mAgent = this;
        mStateMachine.AddState<Idle>(new Idle(), "Idle");
        mStateMachine.AddState<Find>(new Find(), "Find");
        mStateMachine.AddState<Pursuit>(new Pursuit(), "Pursuit");
        mStateMachine.ChangeState("Idle");
    }

    protected virtual void Update()
    {
        if (BattleManager.Instance.status != BattleManager.GameStatus.None
    || PlayerController.Instance.Interaction)
        {
            mStateMachine.ChangeState("Idle");
            return;
        }
        else
            mStateMachine.ActivateState();
        mSpriteRenderer.flipX = (mVelocity.x < -0.001f) ? true : false;
    }

    public virtual void ChangeBehavior(string name)
    {
        mStateMachine.ChangeState(name);
    }
}
