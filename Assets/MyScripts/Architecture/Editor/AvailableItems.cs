using UnityEngine;
using System.Collections.Generic;
using InventoryDiablo;

namespace ModularEventArchitecture
{
    [CreateAssetMenu(fileName = "AvailableItems", menuName = "Inventory/Available Items")]
    public class AvailableItems : ScriptableObject
    {
        public List<InventoryItem> items;
    }
}