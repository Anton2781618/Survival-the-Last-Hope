using System;
using UnityEngine;

namespace AntonStateMachine
{
    public abstract class BaseState<T> where T : Enum
    {
        public T StateKey { get; private set; }

        public BaseState(T key)
        {
            StateKey = key;
        }

        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract void SetNextState();
    }


    public class StateNoWeapon : BaseState<PlayerStateMachie.WeaponStates>
    {
        protected PlayerStateMachie cotext;

        public StateNoWeapon(PlayerStateMachie cotext, PlayerStateMachie.WeaponStates key) : base(key)
        {
        }

        public override void EnterState()
        {
            Debug.Log("EnterState");
        }

        public override void ExitState()
        {
            Debug.Log("ExitState");
        }

        public override void SetNextState()
        {
            cotext.TransitionToState(PlayerStateMachie.WeaponStates.WeaponOn);
        }

        public override void UpdateState()
        {
            Debug.Log("UpdateState");
        }
    }

    public class StateWeaponOn : BaseState<PlayerStateMachie.WeaponStates>
    {
        protected PlayerStateMachie cotext;

        public StateWeaponOn(PlayerStateMachie cotext, PlayerStateMachie.WeaponStates key) : base(key)
        {
        }

        public override void EnterState()
        {
            Debug.Log("EnterState");
        }

        public override void ExitState()
        {
            Debug.Log("ExitState");
        }

        public override void SetNextState()
        {
            cotext.TransitionToState(PlayerStateMachie.WeaponStates.NoWeapon);
        }

        public override void UpdateState()
        {
            Debug.Log("UpdateState");
        }
    }

}
