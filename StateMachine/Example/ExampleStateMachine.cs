using UnityEngine;
using StateMachine;

[RequireComponent(typeof(State1ExampleState))]
[RequireComponent(typeof(State2ExampleState))]
public class ExampleStateMachine : BaseStateMachine
{
    [HideInInspector] public State1ExampleState state1;
    [HideInInspector] public State2ExampleState state2;

    private void Awake() {
        state1 = GetComponent<State1ExampleState>();
        state2 = GetComponent<State2ExampleState>();
    }

    public void SwitchState(ExampleBaseState state)
    {
        if (state is ExampleBaseState)
        {
            base.SwitchState(state);
        }
        else
        {
            Debug.LogError("State must be of type ExampleBaseState");
        }
    }

}