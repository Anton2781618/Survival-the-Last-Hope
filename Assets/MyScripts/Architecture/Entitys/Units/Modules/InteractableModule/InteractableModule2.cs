using System.Collections;
using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UnitEntity))]
    [RequireComponent(typeof(BoxCollider))][ RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(OutlineSystem.Outline))]
    public class InteractableModule2 : InteractableModule, IInteractable
    {
        //-----------------------------------------------------------
        
        //!-----------------------------------------------------------

        public override void Initialize()
        {
 
 
        }

        public override void Interact(GameEntity interactor)
        {
            
            GlobalEventBus.Instance.Publish(EventsUI.Turn_Inventory_Unit_And_Player, new ShowInventory2EventData
            {
                Player = interactor,
                Unit = Entity
            });
        }
    }
}