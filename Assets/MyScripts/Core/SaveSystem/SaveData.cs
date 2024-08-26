using System;
using InventoryDiablo;
using Unity.Mathematics;

[Serializable]
public class SaveData
{
    public InventorySaveData InventorySaveData;


    public float3 Position;
}

[Serializable]
public class InventorySaveData
{
    public int InventoryCount;
    public string[] ItemDataNames;
    public int[] InventoryItemsAmounts;
    
    public InventorySaveData(Inventory inventory)
    {
        InventoryCount = inventory.GetInventoryItems().Count;

        ItemDataNames = new string[InventoryCount];

        InventoryItemsAmounts = new int[InventoryCount];

        for (int i = 0; i < InventoryCount; i++)
        {
            ItemDataNames[i] = inventory.GetInventoryItems()[i].ItemData.name;

            InventoryItemsAmounts[i] = inventory.GetInventoryItems()[i].Amount;
        }        
    }
}
