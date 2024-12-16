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
                OnInventoryItemChanged?.Invoke(inventoryItem);
            }
        }
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
}
