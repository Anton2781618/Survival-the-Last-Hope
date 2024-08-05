using System.Collections;
using InventoryDiablo;
using MyProject;
using UnityEngine;

namespace MyProject
{
    public class SaveManager : MonoBehaviour
    {
        public Player player;
        
        [ContextMenu("Save")]
        public void Save()
        {
            SaveData saveData = new SaveData
            {
                Position = player.transform.position,

                InventorySaveData = new InventorySaveData(player.InventoryHandler.Inventory)
            };

            BlazeSave.SaveData("SaveData", saveData);
        }
        
        [ContextMenu("Load")]
        public void Load()
        {
            SaveData saveData = BlazeSave.LoadData<SaveData>("SaveData");

            Inventory inventory = new Inventory();

            for (int i = 0; i < saveData.InventorySaveData.InventoryCount; i++)
            {
                ItemData itemData = Resources.Load<ItemData>($"ScriptableObjects/{saveData.InventorySaveData.ItemDataNames[i]}");

                InventoryItem inventoryItem = new InventoryItem(itemData, saveData.InventorySaveData.InventoryItemsAmounts[i]);

                inventory.AddItem(inventoryItem);
            }

            
            player.InventoryHandler.Inventory = inventory;

            player.CharacterControllerPlayer.enabled = false;

            player.transform.position = saveData.Position;

            player.CharacterControllerPlayer.enabled = true;
        }
    }
}


