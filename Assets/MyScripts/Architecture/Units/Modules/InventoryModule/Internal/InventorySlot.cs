using System;
using UnityEngine;
using static InventoryDiablo.ItemData;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventorySlot 
    {
        public Sprite Icon; // иконка предмета
        public GridData2 SlotGrid = new GridData2();
    }
}
