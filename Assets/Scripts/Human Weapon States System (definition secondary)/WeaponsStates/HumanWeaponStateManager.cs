using StarterAssets;
using UnityEngine;
using Weapons;
using Zenject;
using Units;
using System;
using InventoryDiablo;

namespace States
{
    public class HumanWeaponStateManager : IStateService, IWeaponStates
    {
        internal IInventorySystem owner;
        public event Action StateCompleted;
        internal HumanModel playerModel;
        internal StarterAssetsInputs inputs;
        private WeaponState currentState = null; 

        internal StateWeaponOff stateWeaponOff;
        internal StateHeandOnWeapon stateHeandOnWeapon;
        internal StateWeaponOn stateWeaponOn;
        internal StateWeaponAim stateWeaponAim;
        internal StateWeaponShoot stateWeaponShoot;
        internal StateWeaponReload stateWeaponReload;

        public float LerpTime { get ; set;}

        private WeaponBase currentWeapon = null;


        public WeaponBase CurrentWeapon
        {
            get => currentWeapon; 
            set
            {
                currentWeapon = value;

                TransitionTo(stateWeaponOff);
            }
        }


        [Inject] 
        public HumanWeaponStateManager(HumanModel playerModel, StarterAssetsInputs inputs)
        {
            this.playerModel = playerModel;
            
            this.inputs = inputs;

            stateHeandOnWeapon = new StateHeandOnWeapon();

            stateWeaponShoot = new StateWeaponShoot();

            stateWeaponReload = new StateWeaponReload();
            
            stateWeaponOff = new StateWeaponOff();

            stateWeaponOn = new StateWeaponOn();
            
            stateWeaponAim = new StateWeaponAim();

            LerpTime = 0.4f;
        }

        // метод позволяет изменять объект Состояния во время выполнения.
        public void TransitionTo(WeaponState state)
        {
            this.currentState = state;
            
            state.InitState(this);

            // Debug.Log("Переход в состояние " + state);
        }

        public void UpdateMe()
        {
            if(CurrentWeapon == null)
            {
                inputs.weaponOn = false;

                if(currentState != stateWeaponOff) TransitionTo(stateWeaponOff);
            }
            
            currentState.Execute();
        }

        public void StateComplete()
        {
            StateCompleted?.Invoke();
        }


        public void SetWeapon(WeaponBase newWeapon)
        {
            if(CurrentWeapon) return;
            
            CurrentWeapon = newWeapon;

            CurrentWeapon?.HolsterWeapon(playerModel.ToPosition);
        }

        /// <summary>
        /// Checks if the player has no weapons.
        /// </summary>
        /// <returns>True if the player has no weapons, false otherwise.</returns>
        public bool WeaponIsNull()
        {
            return CurrentWeapon == null;
        }

        public void TakeAwayWeapon()
        {
            TransitionTo(stateWeaponOff);

            CurrentWeapon.DestructSelf();
            
            CurrentWeapon = null;
        }

        public WeaponBase GetCurrentWeapon()
        {
            return CurrentWeapon;
        }

        public void SetInventoryOwner(IInventorySystem newOwner)
        {
            owner = newOwner;
        }
    }
}
