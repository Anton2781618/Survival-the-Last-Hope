using StarterAssets;
using Weapons;
using Zenject;
using Units;
using InventoryDiablo;
using UnityEngine;

namespace States
{
    public class HumanWeaponStateManager : IStateService, IWeaponStates
    {
        internal IInventorySystem owner;
        internal HumanModel playerModel;
        internal StarterAssetsInputs inputSystem;
        private WeaponState currentState = null; 

        internal StateWeaponOff stateWeaponOff;
        internal StateHeandOnWeapon stateHeandOnWeapon;
        internal StateWeaponOn stateWeaponOn;
        internal StateWeaponAim stateWeaponAim;
        internal StateWeaponChange stateWeaponChange;
        internal StateWeaponShoot stateWeaponShoot;
        internal StateWeaponReload stateWeaponReload;

        public float LerpTime { get ; set;}

        private WeaponBase currentWeapon = null;
        private WeaponBase pistolSlot = null;
        private WeaponBase rifleSlot = null;


        public WeaponBase CurrentWeapon
        {
            get => currentWeapon; 
            set => currentWeapon = value;
        }


        [Inject] 
        public HumanWeaponStateManager(HumanModel playerModel, StarterAssetsInputs inputs)
        {
            this.playerModel = playerModel;
            
            this.inputSystem = inputs;

            stateHeandOnWeapon = new StateHeandOnWeapon();

            stateWeaponShoot = new StateWeaponShoot();

            stateWeaponReload = new StateWeaponReload();
            
            stateWeaponOff = new StateWeaponOff();

            stateWeaponOn = new StateWeaponOn();
            
            stateWeaponAim = new StateWeaponAim();

            stateWeaponChange = new StateWeaponChange();

            LerpTime = 0.4f;

            TransitionTo(stateWeaponOff);
        }

        // метод позволяет изменять объект Состояния во время выполнения.
        public void TransitionTo(WeaponState state)
        {
            this.currentState = state;
            
            state.InitState(this);

            Debug.Log("Переход в состояние " + state);
        }

        public void UpdateMe()
        {
            if(CurrentWeapon == null)
            {
                inputSystem.weaponOn = false;
                
                if(currentState != stateWeaponChange && currentState != stateWeaponOff) TransitionTo(stateWeaponOff);
            }
            
            currentState.Execute();

            if(Input.GetKeyUp(KeyCode.Alpha1))
            {
                if(currentWeapon == pistolSlot)
                {
                    inputSystem.weaponOn = !inputSystem.weaponOn;

                    return;
                } 

                inputSystem.weaponOn = false;

                inputSystem.weaponChange = true;

                stateWeaponChange.SetNewWeapon(pistolSlot);
            }
            else
            if(Input.GetKeyUp(KeyCode.Alpha2))
            {
                if(currentWeapon == rifleSlot)
                {
                    inputSystem.weaponOn = !inputSystem.weaponOn;
                    
                    return;
                }

                inputSystem.weaponOn = false;

                inputSystem.weaponChange = true;

                stateWeaponChange.SetNewWeapon(rifleSlot);
            }

        }

        public void SetWeapon(WeaponBase newWeapon)
        {
            if(newWeapon.TypeWeapon == WeaponBase.WeaponType.Pistol)
            {
                if(pistolSlot == null)
                {
                    pistolSlot = newWeapon;
                }
                else
                {
                    pistolSlot.DestructSelf();
                    pistolSlot = newWeapon;
                }

                newWeapon.HolsterWeapon(playerModel.PistolHolster);
            }
            else
            {
                if(rifleSlot == null)
                {
                    rifleSlot = newWeapon;
                }
                else
                {
                    rifleSlot.DestructSelf();
                    rifleSlot = newWeapon;
                }

                newWeapon.HolsterWeapon(playerModel.RifleHolster);
            }
            
            if(CurrentWeapon) return;

            CurrentWeapon = newWeapon;
        }

        /// <summary>
        /// Проверяет, есть ли у игрока оружие.
        /// </summary>
        /// <returns>True if the player has no weapons, false otherwise.</returns>
        public bool WeaponIsNull() => CurrentWeapon == null;

        public void TakeAwayWeapon(InventoryItem item)
        {
            if(item == CurrentWeapon.InventoryItem)
            {
                TransitionTo(stateWeaponOff);

                CurrentWeapon = null;
            }
            
            ClearSlot(item, pistolSlot);

            ClearSlot(item, rifleSlot);
        }

        private void ClearSlot(InventoryItem item, WeaponBase slot)
        {
            if(slot != null && item == slot.InventoryItem)
            {
                slot.DestructSelf();

                slot = null;
            }
        }

        public void SetInventoryOwner(IInventorySystem newOwner) => owner = newOwner;
    }
}
