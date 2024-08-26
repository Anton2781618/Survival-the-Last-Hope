using System.Collections.Generic;
using System.Linq;
using InventoryDiablo;
using Units;
using UnityEngine;

namespace MyProject
{
    public class GameDataBase : MonoBehaviour
    {
        public GameObject text;
        private Dictionary<Collider, Unit> units = new Dictionary<Collider, Unit>();
        private Dictionary<string ,ItemData> items = new Dictionary<string, ItemData>();

        //запускать LoadItems() каждый раз при запуске игры
        private void Awake()
        {
            LoadItems();
        }


        [ContextMenu("Загрузить предметы")]
        public void LoadItems()
        {
            items.Clear();

            var itemsData = Resources.LoadAll<ItemData>("ScriptableObjects");

            foreach (var item in itemsData)
            {
                items.Add(item.name, item);
            }
        }

        //получить итем по имени
        public ItemData GetItemData(string name) => items[name];
        
        //получить итем по индексу
        public ItemData GetItemData(int index) => items.Values.ElementAt(index);

        public void AddUnit(Collider collider, Unit unit)
        {
            units.Add(collider, unit);        
        }

        public void RemoveUnit(Collider collider)
        {
            units.Remove(collider);
        }

        public Unit GetUnit(Collider collider)
        {
            return units[collider];
        }

        [ContextMenu("Показать юниты")]
        public void ShowUnits()
        {
            foreach (var unit in units)
            {
                Debug.Log(unit.Value.name);
            }
        }
    }

}
