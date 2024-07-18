using System;
using InventoryDiablo;
using Weapons;

namespace States
{
    public interface IWeaponStates
    {
        event Action StateCompleted;
        public void UpdateMe();
        public void SetWeapon(WeaponBase item);
        public bool WeaponIsNull();
        public void TakeAwayWeapon();
        public void SetInventoryOwner(IInventorySystem newOwner);

        public WeaponBase GetCurrentWeapon();
    }

}