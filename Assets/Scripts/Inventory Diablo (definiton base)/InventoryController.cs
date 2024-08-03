using UnityEngine;
using Zenject;

namespace InventoryDiablo
{
    public class InventoryController : MonoBehaviour, IInventoryController
    {
        /* [Inject]  */IInventory IInventoryController.Inventory => inventory;
        [Inject] IInventoryUI IInventoryController.InventoryUI {get;}
        [SerializeField]  private Inventory inventory;
    }
}