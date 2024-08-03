using Units;
using UnityEngine;

namespace InventoryDiablo
{

    public class ItemOnstreet : Unit
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private OutlineSystem.Outline _outline;
        [SerializeField] private GameObject text;

        public Collider GetCollider() => _collider;
        public Rigidbody GetRigidbody() => _rigidbody;
        public OutlineSystem.Outline GetOutline() => _outline;


        public InventoryItem GetItem() => _item;

        public void TakeItem(Inventory inventoryModel)
        {       
            inventoryModel.AddItem(new InventoryItem(_item.ItemData, _item.Amount));

            // GameManager.Instance.RemoveUsableObject(gameObject);

            Destroy(this.gameObject);
        }

        //переключить текст
        public void ShowText(bool isShow) => text.gameObject.SetActive(isShow);


        // public void Use(AbstractBehavior applicant)
        // {
        //     ShowOutline(false);
        //     TakeItem(applicant.Chest);
        // }

        private void Dest()
        {
            Destroy(this.gameObject);
        }

        public void SetupItem(InventoryItem item, GameObject textGo)
        {
            _item = item;

            text = textGo;
        }

        void OnMouseEnter()
        {
            _outline.enabled = true;

            ShowText(true);

            text.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        }

        void OnMouseOver()
        {
            // rend.material.color -= new Color(0.1F, 0, 0) * Time.deltaTime;
        }

        void OnMouseExit()
        {
            ShowText(false);

            _outline.enabled = false;
        }
    }
}