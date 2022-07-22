public struct AIBuild
{
    public enum AIProperty
    {
        Offensive,
        Defensive
    }
    public enum AIType
    {
        None,
        Manual,
        Auto
    }
    public enum AITargetPriority
    {
        None,
        AimToHighHealth
    }
    public enum ActionEvent
    {
        None,
        IntroWalk,
        AttackWalk,
        MagicWalk,
        Busy,
        BackWalk,
        DodgeWalk,
        DodgeBack
    }
    public StateMachine stateMachine;
    public AIProperty property;
    public AITargetPriority priority;
    public AIType type;
    public ActionEvent actionEvent;

    public AIBuild(AIType t, bool initialize = true)
    {
        stateMachine = null;
        priority = AITargetPriority.None;
        type = t;
        actionEvent = ActionEvent.IntroWalk;
        property = (AIProperty)UnityEngine.Random.Range(0, 2);
    }

    public void SetActionEvent(ActionEvent action)
    {
        actionEvent = action;
    }

    public void ChangeState(string stateName)
    {
        stateMachine.ChangeState(stateName);
    }

    public void SetBasicStates()
    {
        AddState(new Waiting(), "Waiting");
        AddState(new Standby(), "Standby");
        AddState(new AttackBehavior(), "Attack");
        AddState(new DefendBehavior(), "Defend");
        AddState(new MagicBehavior(), "Magic");
        ChangeState("Waiting");
    }

    public void AddState(State state, string stateName)
    {
        stateMachine.AddState<State>(state, stateName);
    }

    public void Update(bool isAI)
    {
        if (isAI)
            stateMachine.ActivateState();
    }
}
