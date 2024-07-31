using UnityEngine;
using Zenject;

namespace InventoryDiablo
{
    public class InventoryController : IInventoryController
    {
        [Inject] public IInventory Inventory {get; set;}

        [Inject] public IInventoryUI InventoryUI {get;}
    }
}