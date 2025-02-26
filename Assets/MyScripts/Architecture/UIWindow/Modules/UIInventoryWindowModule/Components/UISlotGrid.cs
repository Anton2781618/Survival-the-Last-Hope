using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    public class UISlotGrid : UIItemGrid
    {
        //-----------------------------------------------------------
        [SerializeField] private UIInventoryContainer uIInventoryContainer;

        //-----------------------------------------------------------
        [Header("Блок префабов")]
        //префаб сетки
        [SerializeField] private UIItemGrid _gridPrefab;
        //префаб итема на сетки
        [SerializeField] private UIInventoryItem _itemPrefab;

        //-----------------------------------------------------------
        //пулл слотов
        private List<UIItemGrid> _itemGridsList = new List<UIItemGrid>();
        private List<UIItemGrid> _itemGridsPool = new List<UIItemGrid>();
        
        //-----------------------------------------------------------
        //пулл итемов
        [SerializeField]private List<UIInventoryItem> _itemList = new List<UIInventoryItem>();
        private List<UIInventoryItem> _itemPool = new List<UIInventoryItem>();

        //!-----------------------------------------------------------

        public override void PlaceItem(UIInventoryItem inventoryItem, int posX, int posY)
        {
            CreateGrids(inventoryItem.InventoryItem);

            base.PlaceItem(inventoryItem, posX, posY);
        }

        public override UIInventoryItem SelectIteme(int x, int y)
        {
            DeactiveGrids();

            return base.SelectIteme(x, y);
        }

        //создать сетки
        public void CreateGrids(InventoryItem item)
        {
            Debug.Log("CreateGrids!!!!!!!!!!!!!");
            Tool.Helper.ResetCards(_itemGridsList, _itemGridsPool);

            foreach (var grid in item.Grids)
            {
                UIItemGrid newUIGrid = Tool.Helper.GetFreeCard(_gridPrefab, _itemGridsPool);

                _itemGridsList.Add(newUIGrid);

                newUIGrid.rectTransform.localPosition = new Vector2(grid.Position.x, -grid.Position.y);

                newUIGrid.Setup(grid.Size.x, grid.Size.y);

                CreateItemsOnGrids(newUIGrid);

                newUIGrid.gameObject.SetActive(true);
            }
        }

        //создать итемы на сетке
        private void CreateItemsOnGrids(UIItemGrid Grid)
        {
            Tool.Helper.ResetCards(_itemList, _itemPool);

            foreach (InventoryItem item in Grid.GridDataInfo.activeItems)
            {
                UIInventoryItem newInventoryItem = Tool.Helper.GetFreeCard(_itemPrefab, _itemPool);

                _itemList.Add(newInventoryItem);

                //парент
                newInventoryItem.transform.SetParent(Grid.transform, false);

                newInventoryItem.rectTransform.localPosition = new Vector2(item.OnGridPosition.x, -item.OnGridPosition.y);

                newInventoryItem.Setup(item, Grid, item.Amount);

                newInventoryItem.gameObject.SetActive(true);
            }
        }

        //выключить сетки
        public void DeactiveGrids()
        {
            Debug.Log("DeactiveGrids!!!!!!!!!!!!!");
            foreach (var grid in _itemGridsList)
            {
                grid.gameObject.SetActive(false);
            }
        }
    }
}