using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;

namespace ModularEventArchitecture
{
    [CompatibleUnit(typeof(UIWindow))]
    public class UIInventoryWindowModule : ModuleBase
    {
        //-----------------------------------------------------------
        [Header("Блок настройки окна инвентаря")]
        [SerializeField] private GameObject windowContent;

        //-----------------------------------------------------------
        //пулл контейнеров
        [SerializeField] private UIInventoryContainer _containerPrefabs;
        private List<UIInventoryContainer> _containerList = new List<UIInventoryContainer>();
        private List<UIInventoryContainer> _containerPool = new List<UIInventoryContainer>();

        //-----------------------------------------------------------
        private Inventory _selectInventory;

        //!-----------------------------------------------------------

        public override void Initialize()
        {
            // Entity.Globalevents.Add((EventsUI.Show_window, (data) => OnShowWondow((ShowInventoryEventData)data)));
            Entity.LocalEvents.Subscribe<ShowInventoryEventData>(EventsUI.Show_window, OnShowWondow);

            Entity.Globalevents.Add((EventsInventory.Item_Spawned_OnGrid, (data) => UpdateInventory((ShowInventoryEventData)data)));
        }

        private void OnShowWondow(ShowInventoryEventData showInventoryEventData)
        {
            Debug.Log("1 " + transform.name);
            if(!windowContent.activeSelf) CreateContainers(showInventoryEventData.Owner.GetModule<InventoryModule>().Inventory);

            ShowInventory(!windowContent.activeSelf);
        }

        //вызывается только когда инвентарь открыт
        private void UpdateInventory(ShowInventoryEventData showInventoryEventData)
        {
            if(!windowContent.activeSelf) return;

            CreateContainers(showInventoryEventData.Owner.GetModule<InventoryModule>().Inventory);
        }

        public void CreateContainers(Inventory inventory)
        {
            _selectInventory = inventory;            

            Tool.Helper.ResetCards(_containerList, _containerPool);

            foreach (InventoryContainer container in _selectInventory.InventoryContainers)
            {
                var newContainer = Tool.Helper.GetFreeCard(_containerPrefabs, _containerPool);

                newContainer.Setup(container, inventory);

                _containerList.Add(newContainer);

                newContainer.gameObject.SetActive(true);
            }
        }

        public void ShowInventory(bool value) => windowContent.SetActive(value);

        //вызыватся снаружи кнопкой 
        public void CreateRandomItem() => GlobalEventBus.Instance.Publish(EventsInventory.Item_Spawned_On_Cursor, new EventBase());
    }
}