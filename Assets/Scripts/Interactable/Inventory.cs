using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryItem;

    public GameObject InventoryItem
    {
        get { return inventoryItem; }
        set
        {
            if (value != inventoryItem)
            {
                inventoryItem = value;
                if (inventoryItem != null)
                {
                    if(inventoryItem.TryGetComponent(out PickableItem pickableItem))
                    {
                        pickableItem.owner = this;
                    }
                }
                OnInventoryItemChanged?.Invoke(inventoryItem);
            }
        }
    }

    public T GetInventoryItemComponent<T>() where T : Component
    {
        if (inventoryItem != null)
        {
            return inventoryItem.GetComponent<T>();
        }
        return null;
    }

    public bool TryGetInventoryItemComponent<T>(out T component) where T : Component
    {
        component = GetInventoryItemComponent<T>();
        return component != null;
    }

    public bool hasItem()
    {
        return inventoryItem != null;
    }

    public bool isEmpty()
    {
        return inventoryItem == null;
    }

    public GameObject DropItem()
    {
        Debug.Log("Dropping item");
        if (inventoryItem != null)
        {
            Debug.Log("Item is not null");
            if(inventoryItem.TryGetComponent(out PickableItem pickableItem))
            {
                pickableItem.owner = null;
            }
            GameObject item = inventoryItem;
            InventoryItem = null;
            return item;
        }
        return null;
    }
    public static void SwapItems(Inventory item1, Inventory item2)
    {
        GameObject temp1 = item1.InventoryItem;
        item1.InventoryItem = item2.InventoryItem;
        item2.InventoryItem = temp1;
    }

    public void swapItems(Inventory item)
    {
        SwapItems(this, item);
    }

    public delegate void InventoryItemChanged(GameObject newItem);
    public event InventoryItemChanged OnInventoryItemChanged;

    void Start()
    {
        if (OnInventoryItemChanged != null)
        {
            OnInventoryItemChanged.Invoke(inventoryItem);
        }

    }

    // This method is called when the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        if (OnInventoryItemChanged != null)
        {
            OnInventoryItemChanged.Invoke(inventoryItem);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static implicit operator Inventory(TableScript v)
    {
        throw new NotImplementedException();
    }
}
