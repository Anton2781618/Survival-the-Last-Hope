using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UIManager))]
    public class ScreensModule : ModuleBase
    {
        //-----------------------------------------------------------
        [SerializeField] private UIWindow _playerInventoryWindow = new UIWindow();
        [SerializeField] private UIWindow _UnitInventoryWindow = new UIWindow();

        //!-----------------------------------------------------------

        public override void Initialize()
        {
            Entity.Globalevents.Add((EventsUI.Turn_Inventory_Player, (data) => OnTurnPlayerInventory((ShowInventoryEventData)data)));
            Entity.Globalevents.Add((EventsUI.Turn_Inventory_Unit_And_Player, (data) => OnTurnInventoryUnitAndPlayer((ShowInventory2EventData)data)));
        }

        private void OnTurnPlayerInventory(ShowInventoryEventData showInventoryEventData)
        {
            _playerInventoryWindow.LocalEvents.Publish(EventsUI.Show_window, new ShowInventoryEventData
            {
                Owner = showInventoryEventData.Owner
            });
        }
        private void OnTurnInventoryUnitAndPlayer(ShowInventory2EventData showInventoryEventData)
        {
            _playerInventoryWindow.LocalEvents.Publish(EventsUI.Show_window, new ShowInventoryEventData
            {
                Owner = showInventoryEventData.Player
            });

            _UnitInventoryWindow.LocalEvents.Publish(EventsUI.Show_window, new ShowInventoryEventData
            {
                Owner = showInventoryEventData.Unit
            });
        }
    }
}