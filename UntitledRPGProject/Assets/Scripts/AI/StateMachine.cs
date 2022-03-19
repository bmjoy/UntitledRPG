using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // Start is called before the first frame update
    public State mCurrentState;
    public Dictionary<string, object> mStates = new Dictionary<string, object>();
    public GameObject mAgent;

    private void Awake()
    {
        mAgent = this.gameObject;
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
