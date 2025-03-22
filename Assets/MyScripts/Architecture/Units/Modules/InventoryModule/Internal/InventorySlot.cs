using System;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventorySlot 
    {
        //---------------------------------------------------
        public Inventory Inventory;
        //---------------------------------------------------
        public Sprite Icon; // иконка предмета
        public bool ShowGrid = true;
        //---------------------------------------------------
        //объект внутри которого будет создаваться предмет помещенный в слот
        public Transform SlotObjectToSpawn;
        //---------------------------------------------------
        public GridData2 SlotGrid = new GridData2();

        //!---------------------------------------------------
    }
}
