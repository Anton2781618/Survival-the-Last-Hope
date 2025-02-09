using System;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Атрибут для отображения массива или списка строк как выпадающего списка
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PopupAttribute : PropertyAttribute
    {       
        public PopupAttribute()
        {
        }
    }
}
