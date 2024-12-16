using Unity.VisualScripting;
using UnityEngine;


public class Interactable : MonoBehaviour {
    public virtual bool Interact(Inventory item) {
        return false;
    }
    public virtual bool tryInteract(Inventory item) {
        return false;
    }
}


public class CoffeMachine : Interactable
{
    public SphereCollider coffeMachineInteractionDistance;
    
    private Inventory inventory;
    private Transform slot;

    [SerializeField]
    public enum CoffeMachineState
    {
        Empty,
        CoffeInside,
        Brewed,
    }

    public CoffeMachineState state;

    private float brewTime = 5f;
    private float brewTimer = 0f;

    public override bool Interact(Inventory item)
    {
        if (inventory.InventoryItem == null && item.InventoryItem != null)
        {
            inventory.swapItems(item);
            return true;
        }
        else if (inventory.InventoryItem != null && item.InventoryItem == null)
        {
            inventory.swapItems(item);
            return true;
        }
        return false;
    }

    public override bool tryInteract(Inventory item)
    {
        return true;
    }

    void Run(Inventory item) {
        if (state == CoffeMachineState.CoffeInside)
        {
            brewTimer = brewTime;
        }
        if (state == CoffeMachineState.Brewed)
        {
            // if interacted, put brewed item in inventory
            
            // get Prefab from prefab/items/brewed
            // instantiate prefab
            // set item state to empty

            var newItem = Instantiate(Resources.Load<GameObject>("Prefabs/Items/CoffeBrewd"));
            inventory.InventoryItem = newItem;

        }
        if (state == CoffeMachineState.Empty)
        {
            // set item state to coffeInside
        }
    }   

    void Start()
    {
        coffeMachineInteractionDistance = GetComponent<SphereCollider>();
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
                // if has rigidbody, disable it
                Rigidbody rb = newItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

            } else {
                slot.DetachChildren();
            }
        };

    }

    // Update is called once per frame
    void Update()
    {
        if (brewTimer > 0)
        {
            brewTimer -= Time.deltaTime;
            if (brewTimer <= 0)
            {
                // set item state to brewed
            }
        }

    }
}
