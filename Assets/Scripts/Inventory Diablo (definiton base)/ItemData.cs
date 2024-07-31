using System;
using UnityEngine;

namespace InventoryDiablo
{

    [CreateAssetMenu, Serializable]
    public class ItemData : ScriptableObject
    {
        public Sprite ItemIcon;
        public string Title;
        public string Description;
        public ItemOnstreet Prefab;

        //показатель пользы. Например для зелья лечения это сколько хп востановит, а для брони это какая защита

        public int Width = 1;
        public int Height = 1;


        //этот префаб одежды которая наденется непосредственно на
        // public GameObject prefabForPutOn;
        
        public int Benefit = 0;
        public bool IsSingle = true; 
        public int MaxAmount = 1;

        //список типов предметов которые могут быть совмещены с этим предметом
        // public ItemType canBeCombinedWith;
        public ItemType TypeItem;
        public ItemData[] CanBeCombined;

        [Flags]
        public enum ItemType
        {
            Шлем = 1 << 0, 
            Броня = 1 << 1, 
            Ремень = 1 << 2, 
            Штаны = 1 << 3,
            Сапоги = 1 << 4,
            Оружие = 1 << 5,
            Щит = 1 << 6,
            Кольцо = 1 << 7,
            Ожерелье = 1 << 8,
            Наплечники = 1 << 9,
            Аптечка = 1 << 10,
            Зелье_маны = 1 << 11,
            Налчные_деньги = 1 << 12,
            Еда = 1 << 13,
            Обойма_патронов = 1 << 14,
            Патроны = 1 << 15,
            Коробка_патронов = 1 << 16,
            
        }

        public int GetItemTypeIndex()
        {
            return TypeItem switch
            {
                ItemType.Шлем => 0,
                ItemType.Броня => 1,
                ItemType.Ремень => 2,
                ItemType.Штаны => 3,
                ItemType.Сапоги => 4,
                ItemType.Оружие => 5,
                ItemType.Щит => 6,
                ItemType.Кольцо => 7,
                ItemType.Ожерелье => 8,
                ItemType.Наплечники => 9,
                ItemType => throw new ArgumentException("Передан недопустимый аргумент")
            };
        }
    }
}