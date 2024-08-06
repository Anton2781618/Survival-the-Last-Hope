using System.Collections.Generic;
using InventoryDiablo;
using UnityEngine;
using Zenject;
using static InventoryDiablo.ItemData;


namespace MyProject
{
    public static class Helper
    {
        [Inject] public static Spawner Spawner;
        [Inject] public static GameDataBase GameDataBase;

        /// <summary>
        /// Выключить все объекты которые передадут в метод, затом перенести их в пул  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cardsList"></param>
        /// <param name="cardsPool"></param>
        public static void ResetCards<T>(List<T> cardsList, List<T> cardsPool) where T : MonoBehaviour
        {
            cardsList.ForEach(card => card.gameObject.SetActive(false));

            cardsPool.AddRange(cardsList);

            cardsList.Clear();
        }

        // получить из пула свободную карточку
        public static T GetFreeCard<T>(T cardsPrfab, List<T> CardsPool) where T : MonoBehaviour
        {
            T result;

            if (CardsPool.Count > 0)
            {
                result = CardsPool[0];

                while (CardsPool.Remove(result)) ;
            }
            else
            {
                result = UnityEngine.Object.Instantiate(cardsPrfab, cardsPrfab.transform.parent);
            }

            return result;
        }

         //получить мировые координаты точки мышки
        private static Vector3 GetMouseWold(Camera cameraMain, Vector3 fromPoint)
        {
            Vector3 mouseWorldPosition = Vector3.zero;

            Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hit, 50f))
            {
                mouseWorldPosition = hit.point;
            }
            
            Vector3 aimDirection = (mouseWorldPosition - fromPoint).normalized;

            return aimDirection;
        }

        //проверить есть ли флаг в перечислении
        public static bool IsHasFlag(ItemType itemType, ItemType flag)
        {
            return itemType.HasFlag(flag);
        }

        //проверить есть ли флаг в перечислении вариант 2
        public static bool AreHasFlag(ItemType itemType, ItemType flag)
        {
            return (itemType & flag) == 0;
        }

        // можно ли в overlapItem поместить inventoryItem(есть ли соответствующий слот у итема)
        private static bool CanBeCombinedItems(UIInventoryItem inventoryItem, UIInventoryItem overlapItem) => overlapItem.InventoryItem.ItemData.TypeItem.HasFlag(inventoryItem.InventoryItem.ItemData.TypeItem);
    }
}
