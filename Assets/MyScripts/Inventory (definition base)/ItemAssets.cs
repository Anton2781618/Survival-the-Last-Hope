using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InventoryDefinition.Item;

namespace InventoryDefinition
{
    public class ItemAssets : MonoBehaviour
    {
        public static ItemAssets Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        
        public Sprite pistolSprite;
        public Sprite rifleSprite;
        public Sprite moneySprite;

        public Sprite GetSprite(ItemType itemType)
        {
            switch (itemType)
            {
                default:
                case ItemType.Pistol: return pistolSprite;
                case ItemType.Rifle: return rifleSprite;
                case ItemType.Money: return moneySprite;
            }
        }

    }
}