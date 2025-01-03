using UnityEngine;
using Weapons;

// этот скрипт для установки позиций оружия
// в редакторе можно установить позицию оружия в руках игрока
// и сохранить ее в ассет оружия
// таким образом можно устанавливать позиции оружия в руках игрока
// для разных оружий
public class ConfigScript : MonoBehaviour
{
    [SerializeField] private Config config;
    [SerializeField] protected WeaponModel weaponModel;
    [SerializeField] private Transform rightHend;

    [SerializeField] private string weaponAnimationName;
    [SerializeField] private int stage;

    public enum Config
    {
        rightHend,
        leftHend
    }



    [InventoryDiablo.Button]
    public void SetPositionsHand()
    {
        if(config == Config.rightHend)
        {
            weaponModel.AnimationsLayers.Find(x => x.Name == weaponAnimationName).AnimationsPoints[stage].SetPositionsRightHand(rightHend);
        }
        else
        {
            weaponModel.AnimationsLayers.Find(x => x.Name == weaponAnimationName).AnimationsPoints[stage].SetPositionsLeftHand(rightHend);
        }
    }
}
