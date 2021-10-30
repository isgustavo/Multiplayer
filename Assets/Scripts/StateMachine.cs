using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateMachine
{
    protected Dictionary<string, State> states;

    public State PreviousState { get; private set; }
    public State CurrentState { get; private set; }

    public StateMachine (Dictionary<string, State> states)
    {
        this.states = states;
    }

    public StateMachine (Dictionary<string, State> states, string initialState)
    {
        this.states = states;
        ChangeState(initialState);
    }

    public void UpdateState ()
    {
        CurrentState?.UpdateState();
    }

    public void FixedUpdateState ()
    {
        CurrentState?.FixedUpdateState();
    }

    public void LateUpdateState ()
    {
        CurrentState?.LateUpdateState();
    }

    public void ChangeState (string state)
    {
        if (CurrentState?.GetType().Name == state)
            return;

        if (states.ContainsKey(state))
        {
            CurrentState?.OnLeaveState();
            PreviousState = CurrentState;
            State nextState = states[state];
            nextState.OnEnterState(PreviousState);
            CurrentState = nextState;
        }
        else
        {
            Debug.LogError($"State not found {state}");
        }
    }
}
