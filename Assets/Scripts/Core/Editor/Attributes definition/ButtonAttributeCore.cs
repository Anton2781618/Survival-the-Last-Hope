using System;

namespace EasyButtons
{
    public enum ButtonModeType
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
    public sealed class ButtonAttributeAntonAttributeCore : Attribute
    {
        public ButtonModeType mode;
        public ButtonAttributeAntonAttributeCore(ButtonModeType mode = ButtonModeType.AlwaysEnabled)
        {
            this.mode = mode;
        }
    }
}
