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
                Debug.Log("InventoryItem set via property.");
                OnInventoryItemChanged?.Invoke(inventoryItem);
            }
        }
    }

    public delegate void InventoryItemChanged(GameObject newItem);
    public event InventoryItemChanged OnInventoryItemChanged;

    void Start()
    {
        Debug.Log("Start method called.");
        if (OnInventoryItemChanged != null)
        {
            Debug.Log("OnInventoryItemChanged has subscribers.");
            OnInventoryItemChanged.Invoke(inventoryItem);
        }
        else
        {
            Debug.Log("OnInventoryItemChanged has no subscribers.");
        }
    }

    // This method is called when the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        Debug.Log("OnValidate method called.");
        if (OnInventoryItemChanged != null)
        {
            Debug.Log("OnInventoryItemChanged has subscribers.");
            OnInventoryItemChanged.Invoke(inventoryItem);
        }
        else
        {
            Debug.Log("OnInventoryItemChanged has no subscribers.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
