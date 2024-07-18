using UnityEngine;

namespace InventoryDefinition
{
    public class ItemWorld : MonoBehaviour
    {
        public Item item;
        public Rigidbody rb;
        [SerializeField] private OutlineSystem.Outline outline;

        public void SetupItem(Item item)
        {
            this.item = item;
        }

        // The mesh goes red when the mouse is over it...
        void OnMouseEnter()
        {
            Debug.Log("OnMouseEnter");

            outline.enabled = true;
        }

        void OnMouseOver()
        {
            // rend.material.color -= new Color(0.1F, 0, 0) * Time.deltaTime;
        }

        void OnMouseExit()
        {
            Debug.Log("OnMouseExit");

            this.outline.enabled = false;
        }
    }
}