using System;
using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventorySlot 
    {
        public Sprite Icon; // иконка предмета
        public GridData.GridInfo SlotName = GridData.GridInfo.BackpackSlot;
        public ItemType TypeItem = ItemType.Шлем; // тип предмета который можно установить в слот
        public GridData2 Grids2 = new GridData2();
        // public InventoryItem SlotItem = new InventoryItem(); // предмет в слоте
    }
}
