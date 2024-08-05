using InventoryDiablo;
using Units;
using UnityEngine;
using Zenject;

public class Chest : Unit, IInventorySystem
{
    [SerializeField] private OutlineSystem.Outline _outline;
    [SerializeField] private GameObject text;

    [Inject] public InventoryHandler InventoryHandler { get; set; }

    private void Start() 
    {
    }

    public override void Use(Unit unit)
    {

        InventoryHandler.InventoryUI.SetInventoryOwner(this);
        
        IInventorySystem unitInventory = unit as IInventorySystem;

        unitInventory.InventoryHandler.InventoryUI.ShowInventory();

        InventoryHandler.InventoryUI.ShowInventory();
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
