using System;
using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;
using Zenject;

namespace ModularEventArchitecture
{
    [Serializable]
    public class InventorySlot 
    {
        public InventoryItem SlotItem; // предмет в слоте
        [field: SerializeField] public List<InventoryItem> SetupItems {get; set;}
        

        public void TryPlaceItems() 
        {
            if(SlotItem.Grids == null)
            {
                SlotItem.InitGrid();
            }

            // Очищаем сетку перед размещением
            foreach (var grid in SlotItem.Grids)
            {
                grid.Clear();
            }
            
            // Пробуем разместить каждый предмет
            foreach (var item in SetupItems) 
            {
                bool placed = false;
                if (item?.ItemData == null)
                {
                    Debug.Log("Не указаны начальные предметы для слота инвентаря!!!!!");

                    continue;
                }

                Vector2Int? posOnGrid;

                foreach (var grid in SlotItem.Grids)
                {
                    posOnGrid = grid.FindSpaceForObject(item);

                    // Ищем место для предмета
                    if (posOnGrid != null)
                    {
                        grid.PlaceItem(item, posOnGrid.Value.x, posOnGrid.Value.y);

                        Debug.Log($"Предмет {item.ItemData.Title} размещен в {grid.GridName} на позиции {posOnGrid.Value.x}, {posOnGrid.Value.y}");

                        placed = true;

                        break;
                    }
                }
                
                if (!placed)
                {
                    Debug.LogWarning($"Не удалось разместить предмет {item.ItemData.Title} - нет места!");

                    return;
                }
            }

            Debug.Log("Размещение предметов завершено успешно!");
        }
    }
}
