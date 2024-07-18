using System.Collections.Generic;
using UnityEngine;
using System;

namespace AntonStateMachine
{
    public abstract class StateManager<T> : MonoBehaviour where T : Enum
    {
        protected Dictionary<T, BaseState<T>> states = new Dictionary<T, BaseState<T>>();
        protected BaseState<T> currentState;

        private void Start() => currentState.EnterState();
        private void Update()
        {
            currentState.UpdateState();
        }

        public abstract void InitializeStates();
         

        public void TransitionToState(T nextStateKey)
        {
            currentState.ExitState();

            currentState = states[nextStateKey];
        }
    }
}
