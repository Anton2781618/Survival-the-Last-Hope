using InventoryDiablo;
using Units;
using UnityEngine;

public class Chest : Unit, IInventorySystem
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private OutlineSystem.Outline _outline;
    [SerializeField] private GameObject text;

    public InventoryHandler InventoryHandler { get; set; }

    public override void Use()
    {
        
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

    public void ShowText(bool isShow) => text.gameObject.SetActive(isShow);

    public void EquipItem(InventoryItem item)
    {
        throw new System.NotImplementedException();
    }

    public void TakeOffItem(UIInventoryItem item)
    {
        throw new System.NotImplementedException();
    }

    public void DropItem(InventoryItem item)
    {
        throw new System.NotImplementedException();
    }
}
