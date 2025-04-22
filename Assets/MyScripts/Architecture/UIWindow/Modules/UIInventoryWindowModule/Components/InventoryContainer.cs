using System;
using System.Collections.Generic;
using UnityEngine;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventoryContainer 
    {
        //-------------------------------------------------------------------------------------
        public List<InventorySlot> Slots = new List<InventorySlot>();
        public Vector2 Position;
        public ItemType TypeItem = ItemType.Шлем; // тип предмета который можно установить в слот
        public Vector2Int Size = new Vector2Int(830, 750); 
    }
}