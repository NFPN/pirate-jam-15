using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; set; }

    public void Initialize(PlayerState startState)
    {
        CurrentState = startState;
        CurrentState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }
}
