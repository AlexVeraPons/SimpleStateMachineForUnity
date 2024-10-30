using System;
using UnityEngine;

namespace StateMachine
{
    public abstract class BaseStateMachine : MonoBehaviour
    {
        public Action OnChangeState;
        protected BaseState currentState;

        /// <summary>
        /// Switches the current state to the new state.
        /// </summary>
        /// <param name="state"></param>
        public virtual void SwitchState(BaseState state)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = state;
            currentState.OnEnter();
            OnChangeState?.Invoke();
        }

        /// <summary>
        /// Updates the current state.
        /// </summary>
        protected virtual void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate();
            }
        }
    }
}
