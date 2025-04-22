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

    //----------------------------------------------------------
    [ReadOnly] [SerializeField] private string currentAnimationName = "";
    [ReadOnly] [SerializeField] private int currentAnimationStage = 0;
    private int indexAnimation = 0;

    //----------------------------------------------------------
    public enum Config
    {
        rightHend,
        leftHend
    }
    //!----------------------------------------------------------


    [Tools.Button("Следующая анимация")]
    private void SetNextAnimation()
    {

        //выбрать следуующий AnimationsLayers 
        indexAnimation = (indexAnimation + 1) % weaponModel.AnimationsLayers.Count;

        currentAnimationName = weaponModel.AnimationsLayers[indexAnimation].Name;
    }

    [Tools.Button("Следующий Этап")]
    private void SetNextStage()
    {
        currentAnimationStage = (currentAnimationStage + 1) % weaponModel.AnimationsLayers[indexAnimation].AnimationsPoints.Count;
    }

    [Tools.Button("Задать позицию")]
    public void SetPositionsHand()
    {
        if(config == Config.rightHend)
        {
            weaponModel.AnimationsLayers.Find(x => x.Name == currentAnimationName).AnimationsPoints[currentAnimationStage].SetPositionsRightHand(rightHend);
        }
        else
        {
            weaponModel.AnimationsLayers.Find(x => x.Name == currentAnimationName).AnimationsPoints[currentAnimationStage].SetPositionsLeftHand(rightHend);
        }
    }
}
