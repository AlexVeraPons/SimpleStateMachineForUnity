using UnityEngine;
using System;

namespace StateMachine
{
    public abstract class BaseState : MonoBehaviour
    {
        public Action OnEntered;
        public Action OnExited;

        [HideInInspector] public bool IsActive;

        public virtual void OnUpdate() { }

        public virtual void OnEnter()
        {
            IsActive = true;
            OnEntered?.Invoke();
        }

        public virtual void OnExit()
        {
            IsActive = false;
            OnExited?.Invoke();
        }
        
        protected virtual void OnDestroy()
        {
            if (IsActive)
            {
                OnExit();
            }

        }
    }
}
