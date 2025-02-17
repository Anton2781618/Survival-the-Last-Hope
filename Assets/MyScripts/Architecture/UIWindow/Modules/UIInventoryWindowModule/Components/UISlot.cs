using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using Tool;
using UnityEngine;

public class UISlotBuilder : MonoBehaviour
{
    //-----------------------------------------------------------
    [Header("Префаб сетки")]
    [SerializeField] private ItemGrid _gridPrefab;

    //-----------------------------------------------------------
    [Header("Блок сетка слота")]
    [SerializeField] private ItemGrid _slotGrid;

    //-----------------------------------------------------------
    //список и пулл
    public List<ItemGrid> _itemGridsList = new List<ItemGrid>();
    private List<ItemGrid> _itemGridsPool = new List<ItemGrid>();
    
    //-----------------------------------------------------------

    public void InsertItemInSlot(InventoryItem item)
    {
        CreateGrids(item);
    }

    private void CreateGrids(InventoryItem item)
    {
        Debug.Log("CreateGrids");
        Tool.Helper.ResetCards(_itemGridsList, _itemGridsPool);

        foreach (var grid in item.Grids)
        {
            ItemGrid newUIGrid = Tool.Helper.GetFreeCard(_gridPrefab, _itemGridsPool);

            _itemGridsList.Add(newUIGrid);

            newUIGrid.rectTransform.localPosition = new Vector2(grid.Position.x, -grid.Position.y);

            newUIGrid.Init(grid.GridSize.x, grid.GridSize.y);

            newUIGrid.gameObject.SetActive(true);
            
        }

    }
    
}
