using System;
using InventoryDiablo;
using Weapons;

namespace States
{
    public interface IWeaponStates
    {
        public void UpdateMe();
        public void SetWeapon(WeaponBase item);
        public bool WeaponIsNull();
        public void TakeAwayWeapon(InventoryItem item);
        public void SetInventoryOwner(IInventorySystem newOwner);
    }

}