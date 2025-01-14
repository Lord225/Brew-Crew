using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : Interactable
{
    private Inventory inventory;
    private Transform slot;
    public ClientScript client;
    public bool IsEmpty { get; private set; } = true;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        slot = transform.Find("Slot");

        inventory.OnInventoryItemChanged += (GameObject newItem) =>
        {
            if (newItem != null)
            {
                newItem.transform.SetParent(slot);
                newItem.transform.localPosition = Vector3.zero;
                newItem.transform.localRotation = Quaternion.identity;
                newItem.transform.localScale = Vector3.one;
                Rigidbody rb = newItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                SetEmpty(false);
                
                // notify client that the order is ready
                if (client != null)
                {
                    client.SetStateToEat();
                }
            }
            else
            {
                slot.DetachChildren();
                SetEmpty(true);
            }
        };
    }

    void Update()
    {
        CheckForProximalItems();
    }

    void CheckForProximalItems()
    {
        if (inventory.InventoryItem != null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
            foreach (var collider in colliders)
            {
                PickableItem pickableItem = collider.GetComponent<PickableItem>();
                if (pickableItem != null && pickableItem.owner == null && IsValidItem(pickableItem.gameObject))
                {
                    inventory.InventoryItem = pickableItem.gameObject;
                }
            }
        }
    }

    public override bool tryInteract(Inventory otherItem)
    {
        if (inventory.isEmpty() && otherItem.hasItem())
        {
            if (IsValidItem(otherItem.InventoryItem))
            {
                inventory.swapItems(otherItem);
                return true;
            }
        }

        return false;
    }

    public override bool Interact(Inventory otherItem)
    {   
        if (inventory.isEmpty() && otherItem.hasItem())
        {
            if (IsValidItem(otherItem.InventoryItem))
            {
                inventory.swapItems(otherItem);
                return true;
            }
        }

        return false;
    }

    private bool IsValidItem(GameObject item)
    {
        if (item.TryGetComponent(out MugState mugState) && client != null)
        {
            return mugState.state == client.order.requestedMug;
        }

        return false;
    }

    public void SetEmpty(bool empty)
    {
        IsEmpty = empty;
        if(empty) {
            Debug.Log("Clearing table"); 
            client.order = null;
            client = null;
            Destroy(inventory.DropItem());
        }
    }
}
