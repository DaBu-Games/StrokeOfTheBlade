using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine
{
    public IState currentState { get; private set; }
    private List<Transition> transitions = new List<Transition>();
    private List<Transition> currentTransitions = new List<Transition>();

    public void OnUpdate()
    {
        foreach (var transition in currentTransitions) 
        {
            if( transition.CheckCondition())
                SwitchState( transition.ToState );
        }
        
        currentState?.OnUpdate();
    }

    public void OnFixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }

    public void SwitchState(IState state)
    {
        currentState?.OnExitState();
        currentState = state;
        if (currentState == null)
            return;
        
        currentState.OnEnterState();
        Debug.Log(currentState.ToString());
        currentTransitions = transitions.FindAll(x => x.FromState == currentState || x.FromState == null);
    }

    public void AddTransition(Transition transition)
    {
        transitions.Add(transition);
    }
}