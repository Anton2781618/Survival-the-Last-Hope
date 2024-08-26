using UnityEngine;
using Weapons;

// этот скрипт для установки позиций оружия
// в редакторе можно установить позицию оружия в руках игрока
// и сохранить ее в ассет оружия
// таким образом можно устанавливать позиции оружия в руках игрока
// для разных оружий
public class ConfigScript : MonoBehaviour
{
    [SerializeField] protected WeaponModel weaponModel;
    [SerializeField] private Transform rightHend;

    [SerializeField] private WeaponStateConfig weaponStateConfig = WeaponStateConfig.Aim;

    enum WeaponStateConfig
    {
        weaponOn,
        Aim,
    }


    [InventoryDiablo.Button]
    public void SetPositionsRightHand()
    {
        weaponModel.WeaponStates[(int)weaponStateConfig].SetPositionsRightHand(rightHend);
    }
}
