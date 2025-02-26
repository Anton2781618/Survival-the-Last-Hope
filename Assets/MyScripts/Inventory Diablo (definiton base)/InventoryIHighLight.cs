using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    //класс занимается подсветкой зоны куда мы будем помещать итем
    [CompatibleUnit(typeof(InventoryManager))]
    public class InventoryIHighLight : ModuleBase
    {
        //---------------------------------------------------
        //сюда ставить объект подсветки который находится внутри инвентаря
        [SerializeField] private RectTransform highLighter;

        //!---------------------------------------------------

        public override void Initialize()
        {
            
        }

        public override void UpdateMe()
        {
            
        }

        //показать подсветку
        public void Show(bool value)
        {
            highLighter.gameObject.SetActive(value);
            
            highLighter.SetAsFirstSibling();
        }

        //устанавить размер подсветки
        public void SetSize(UIInventoryItem targetItem)
        {
            Vector2 size = new Vector2();
            size.x = targetItem.InventoryItem.WIDTH * GridData.titleSizeWidth;
            size.y = targetItem.InventoryItem.HEIGHT * GridData.titleSizeHeight;
            highLighter.sizeDelta = size;
        }

        //установить позицию подсветки
        public void SetPosition(UIItemGrid targetGrid, UIInventoryItem targetItem)
        {
            Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.InventoryItem.OnGridPosition.x, targetItem.InventoryItem.OnGridPosition.y);

            highLighter.localPosition = pos;

        }

        //установить позицию подсветки
        public void SetPosition(UIItemGrid targetGrid, UIInventoryItem targetItem, int posX, int posY)
        {
            Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

            highLighter.localPosition = pos;
        }

        //установить родителя подсветки
        public void SetParent(UIItemGrid targetGrid)
        {
            if(targetGrid == null){return; }
            highLighter.SetParent(targetGrid.GetComponent<RectTransform>());
        }
    }
}