using UnityEngine;

public class MilkSteamerScript : Interactable
{
    private Inventory inventory;
    public GameObject steamedMilkPrefab;
    public Transform slot;
    private IconScript icon;

    public float steamingEnd = 1f;

    public float steamOnUse = 0.3f;

    public float steamDecrease = 0.995f;

    public float steamTimer = 0f;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        icon = GetComponent<IconScript>();

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

    public override bool Interact(Inventory inventory)
    {
        if(this.inventory.TryGetInventoryItemComponent(out Milk milk))
        {
            Use();
            return true;
        }

        if (inventory.InventoryItem == null && this.inventory.InventoryItem != null)
        {
            inventory.swapItems(this.inventory);
            return true;
        }
        else if (inventory.InventoryItem != null && this.inventory.InventoryItem == null)
        {
            inventory.swapItems(this.inventory);
            return true;
        }

        return false;
    }

    public override bool tryInteract(Inventory other) {
        return true;
    }


    public override bool Use()
    {
        // if milk in inventory
        if (inventory.TryGetInventoryItemComponent(out Milk milk))
        {
            // we can steam on use
            steamTimer += steamOnUse;
            
            // if steaming is done
            if (steamTimer >= steamingEnd)
            {
                // remove milk
                Destroy(inventory.DropItem());
                // create steamed milk
                var steamedMilk = Instantiate(steamedMilkPrefab, transform.position, Quaternion.identity);
                // drop steamed milk
                inventory.InventoryItem = steamedMilk;
                // reset timer
                steamTimer = 0;
                return true;
            }

            return true;
        }

        return false;
    }

    void Update()
    {
        if (inventory.InventoryItem != null)
        {
            if (inventory.TryGetInventoryItemComponent(out Milk milk))
            {
                // decrease steam with deltatime
                steamTimer = Mathf.Max(0, steamTimer - steamDecrease * Time.deltaTime);
                icon.showIcon = true;
            } else {
                icon.showIcon = false;
            }
        } else {
            steamTimer = 0;
            icon.showIcon = false;
        }
        
        icon.color = Color.Lerp(Color.white, Color.red, steamTimer / steamingEnd);
    }
}
