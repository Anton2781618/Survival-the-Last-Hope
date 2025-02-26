using System;
using UnityEngine;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventorySlot 
    {
        public Sprite Icon; // иконка предмета
        public ItemType TypeItem = ItemType.Шлем; // тип предмета который можно установить в слот
        public GridData2 SlotGrid = new GridData2();
        public Vector2 Position;
    }
}
