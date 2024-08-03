using UnityEngine;
using Zenject;

namespace InventoryDiablo
{
    public class InventoryHandler : MonoBehaviour, IInventory
    {
        public Inventory Inventory => inventory;
        [Inject] public IInventoryUI InventoryUI {get;}
        [SerializeField] private Inventory inventory;
        
    }
}