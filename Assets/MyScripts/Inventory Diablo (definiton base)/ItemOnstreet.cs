using ModularEventArchitecture;
using UnityEngine;

namespace InventoryDiablo
{
    [RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(OutlineSystem.Outline))]
    public class ItemOnstreet : MonoBehaviour, IInteractable
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private OutlineSystem.Outline _outline;

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

        public void Interact(GameEntity interactor)
        {
            Debug.Log("Поднять предмет");
            bool place = interactor.GetModule<InventoryModule>().Inventory.TryPlaceItem(_item);

            if(!place)
            {
                Debug.Log("Нет места в инвентаре");
                
                return;
            }

            OnMouseExit();

            Destroy(gameObject);
        }

        public bool CanInteract(GameEntity interactor)
        {
            throw new System.NotImplementedException();
        }

        public void SetupItem(InventoryItem item)
        {
            _item = item;
        }

        void OnMouseEnter()
        {
            _outline.enabled = true;

            Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);

            GlobalEventBus.Instance.Publish(EventsUI.Show_Text, new EventShowText 
            {
                Enabled = true,

                Text = _item.ItemData.Title,
            
                Position = position
            });

        }

        void OnMouseOver()
        {
            // rend.material.color -= new Color(0.1F, 0, 0) * Time.deltaTime;
        }

        void OnMouseExit()
        {
            GlobalEventBus.Instance.Publish(EventsUI.Show_Text, new EventShowText 
            {
                Enabled = false,
                
                Text = "",            
                
                Position = Vector3.zero
            });

            _outline.enabled = false;
        }

        
    }
}