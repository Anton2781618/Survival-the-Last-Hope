using UnityEngine;
using TMPro;

namespace InventoryDiablo
{

    //Класс представляет контекстное меню
    public class UIContextMenu : MonoBehaviour
    {
        [SerializeField] private RectTransform _myRect;
        [SerializeField] private TextMeshProUGUI title;
        private UIInventoryItem targetItem;

        public void Setup(UIInventoryItem item)
        {
            targetItem = item;

            SetPositionPopap(_myRect);

            title.text = targetItem.InventoryItem.ItemData.Title;
            
            Show(true);
        }

        public void Show(bool value) => gameObject.SetActive(value);

        private void SetPositionPopap(RectTransform value)
        {
            value.position = Input.mousePosition.x + value.sizeDelta.x > Screen.width ? 
            value.position = new Vector2(Input.mousePosition.x - value.sizeDelta.x, Input.mousePosition.y) : value.position = Input.mousePosition;
        }
    }
}