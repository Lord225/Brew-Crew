using System;
using UnityEngine;

public class DropperScript : Interactable
{
    public GameObject itemPrefab;

    void Start()
    {
        
    }


    void Update()
    {

    }

    public override bool Interact(Inventory inventory)
    {
        if (itemPrefab != null && inventory.InventoryItem == null)
        {
            var itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            inventory.InventoryItem = itemInstance;
            return true;
        }

        return false;
    }

    public override bool tryInteract(Inventory item) {
        if (item.InventoryItem == null) {
            return true;
        }
        return false;
    }

    // Run 
    public override bool Use()
    {
        return false;
    }
}
