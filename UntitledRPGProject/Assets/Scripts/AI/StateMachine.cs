using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // Start is called before the first frame update
    public State mCurrentState;
    public Dictionary<string, object> mStates = new Dictionary<string, object>();
    public Unit mAgent;
    public Unit mPreferredTarget;

    private void Awake()
    {
        mAgent = this.transform.GetComponent<Unit>();
        mPreferredTarget = null;
    }

    public void AddState<T>(T newState, string stateName) where T : class
    {
        if (mStates.ContainsKey(stateName))
        {
            Debug.Log("<color=red>Warning!</color> The state already exists " + stateName);
            return;
        }
        else if (mStates.ContainsValue(newState))
        {
            Debug.Log("<color=red>Warning!</color> The state already exists " + newState.ToString());
            return;
        }
        else
        {
            mStates.Add(stateName, newState);
            if (mCurrentState == null)
                mCurrentState = mStates[stateName] as State;
        }
    }

    public void ChangeState(string stateName)
    {
        State _state = null;
        if (mStates.ContainsKey(stateName))
        {
            _state = mStates[stateName] as State;
        }
        else
        {
            Debug.Log("<color=red>Warning!</color> The state does not exist on his behavior");
        }

        if (mCurrentState != null)
            mCurrentState.Exit(mAgent);
        mCurrentState = _state;
        mCurrentState.Enter(mAgent);
    }

    public string GetCurrentState()
    {
        return mCurrentState.ToString();
    }

    public void ActivateState()
    {
        if (mAgent == null)
            return;
        if (mCurrentState != null)
            mCurrentState.Execute(mAgent);
    }
}

public class ProwlerStateMachine : MonoBehaviour
{
    // Start is called before the first frame update
    public P_State mCurrentState;
    public Dictionary<string, object> mStates = new Dictionary<string, object>();
    public Prowler mAgent;
    private void Awake()
    {
        mAgent = this.transform.GetComponent<Prowler>();
    }

    public void AddState<T>(T newState, string stateName) where T : class
    {
        if (mStates.ContainsKey(stateName))
        {
            Debug.Log("<color=red>Warning!</color> The state already exists " + stateName);
            return;
        }
        else if (mStates.ContainsValue(newState))
        {
            Debug.Log("<color=red>Warning!</color> The state already exists " + newState.ToString());
            return;
        }
        else
        {
            mStates.Add(stateName, newState);
            if (mCurrentState == null)
                mCurrentState = mStates[stateName] as P_State;
        }
    }

    public void ChangeState(string stateName)
    {
        P_State _state = null;
        if (mStates.ContainsKey(stateName))
        {
            _state = mStates[stateName] as P_State;
        }
        else
        {
            Debug.Log("<color=red>Warning!</color> The state does not exist on his behavior");
        }

        if (mCurrentState != null)
            mCurrentState.Exit(mAgent);
        mCurrentState = _state;
        mCurrentState.Enter(mAgent);
    }

    public string GetCurrentState()
    {
        return mCurrentState.ToString();
    }

    public void ActivateState()
    {
        if (mAgent == null)
            return;
        if (mCurrentState != null)
            mCurrentState.Execute(mAgent);
    }
}