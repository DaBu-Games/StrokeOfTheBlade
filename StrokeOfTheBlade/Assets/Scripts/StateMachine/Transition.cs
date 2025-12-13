using UnityEngine;

public class Transition
{
    public readonly IState FromState;
    public readonly IState ToState;
    private System.Func<bool> _condition;

    public Transition(IState fromState, IState toState, System.Func<bool> condition)
    {
        FromState = fromState;
        ToState = toState;
        _condition = condition;
    }

    public bool CheckCondition()
    {
        return _condition();
    }
}