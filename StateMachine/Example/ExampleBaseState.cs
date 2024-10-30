using UnityEngine;
using StateMachine;

public abstract class ExampleBaseState : BaseState
{
    protected ExampleStateMachine stateMachine;
    private void Awake() {
        stateMachine = GetComponent<ExampleStateMachine>();
    }
}