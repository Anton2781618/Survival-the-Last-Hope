using System;

namespace InventoryDiablo
{
    public enum ButtonMode
    {
        AlwaysEnabled,
        EnabledInPlayMode,
        DisabledInPlayMode
    }
    /// <summary>
    /// Атрибут для создания кнопки в инспекторе для вызова метода, к которому она привязана.
    /// Метод должен быть общедоступным и не иметь аргументов.
    /// </summary>
    /// <example>
    /// [Button]
    /// public void MyMethod()
    /// {
    ///     Debug.Log("Clicked!");
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ButtonAttribute : Attribute
    {
        public ButtonMode mode;
        public ButtonAttribute(ButtonMode mode = ButtonMode.AlwaysEnabled)
        {
            this.mode = mode;
        }
    }
}
