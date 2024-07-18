using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryDefinition
{
    [Serializable]
    public class Item
    {
        public string Name;
        public string Description;
        public Sprite Icon;

        public ItemWorld Prefab;
        public ItemType itemType;
        public int amount;

        public enum ItemType
        {
            Money,
            Pistol,
            Rifle,
        }

        public bool IsStackable()
        {
            switch (itemType)
            {
                default:
                case ItemType.Money:
                    return true;
                case ItemType.Pistol:
                    return false;
                case ItemType.Rifle:
                    return false;
            }
        }
        
    
    }
}
