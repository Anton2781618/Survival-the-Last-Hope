using UnityEngine;
using Zenject;

namespace InventoryDiablo
{
    public class InventoryHandler : MonoBehaviour, IInventory
    {
        public Inventory Inventory { get => inventory; set => inventory = value; }
        [Inject] public IInventoryUI InventoryUI {get;}
        [SerializeField] private Inventory inventory;
        
    }
}