using System.Collections.Generic;
using InventoryDiablo;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


namespace ModularEventArchitecture
{
    public class UISlotGrid : UIItemGrid
    {
        //-----------------------------------------------------------
        public UnityEngine.UI.Image Icon;

        //-----------------------------------------------------------
        [Header("Блок префабов")]
        //префаб сетки
        [SerializeField] private UIItemGrid _gridPrefab;

        //-----------------------------------------------------------
        //пулл сеток которые находятся внутри итема лежащего в слоте
        private List<UIItemGrid> _UIitemGridsList = new List<UIItemGrid>();
        private List<UIItemGrid> _UIitemGridsPool = new List<UIItemGrid>();
        
        //-----------------------------------------------------------
        //пулл итемов которые находятся внутри всех сеток _UIitemGridsList
        private List<UIInventoryItem> _UIInventoryItemList = new List<UIInventoryItem>();
        private List<UIInventoryItem> _UIInventoryItemPool = new List<UIInventoryItem>();

        //!-----------------------------------------------------------

        public override void PlaceItem(UIInventoryItem UIInventoryItem, int posX, int posY)
        {
            CreateGridsForItems(UIInventoryItem.InventoryItem);

            base.PlaceItem(UIInventoryItem, posX, posY);
        }

        public override UIInventoryItem SelectIteme(int x, int y)
        {
            DeactiveGrids();
            
            Tool.Helper.ResetCards(_UIInventoryItemList, _UIInventoryItemPool);

            return base.SelectIteme(x, y);
        }

        //создать сетки которые находятся в итеме который положиле в слот
        public void CreateGridsForItems(InventoryItem item)
        {
            Tool.Helper.ResetCards(_UIitemGridsList, _UIitemGridsPool);

            foreach (var itemGrid in item.Grids)
            {
                UIItemGrid newUIGrid = Tool.Helper.GetFreeCard(_gridPrefab, _UIitemGridsPool);

                _UIitemGridsList.Add(newUIGrid);

                newUIGrid.RectTransform.localPosition = new Vector2(itemGrid.Position.x, -itemGrid.Position.y);

                newUIGrid.Setup(itemGrid);

                newUIGrid.gameObject.SetActive(true);
            }
        }

        //выключить сетки
        public void DeactiveGrids()
        {
            Debug.Log("DeactiveGrids!!!!!!!!!!!!!");
            foreach (var grid in _UIitemGridsList)
            {
                grid.DeactiveGrid();
            }
        }
    }
}