using UnityEngine;
using Zenject;

namespace InventoryDiablo
{
    public class InventoryController : IInventoryController
    {
        [Inject] public IInventory Inventory {get;}

        [Inject] public IInventoryUI InventoryUI {get;}
    }
}