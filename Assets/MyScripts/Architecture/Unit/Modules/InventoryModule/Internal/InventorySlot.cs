using System;
using static InventoryDefinition.Item;
using InventoryDiablo;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventorySlot 
    {
        public ItemType AcceptedType; // тип принимаемых предметов
        public GridData GridData;
    }
}