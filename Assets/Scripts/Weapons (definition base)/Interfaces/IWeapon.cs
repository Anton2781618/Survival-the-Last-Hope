using UnityEngine;
using Weapons;

namespace Weapons
{
    public interface IWeapon
    {
        public abstract void Fire();

        public abstract void StopFire();

        public Transform GetWeaponTransform();

        /// <summary>
        /// убрать оружие в кабуру
        /// </summary>
        public void HolsterWeapon(Transform holster);

        /// <summary>
        /// достать оружие из кабуры
        /// </summary>
        public void DrawWeapon(Transform heand);

        public WeaponModel GetModel();
    }
}