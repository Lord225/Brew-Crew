using System.Collections.Generic;
using UnityEngine;

public class CounterTopScript : Interactable
{
    private Inventory inventory;
    public Transform slot;
    public GameObject mugPrefab;

    void Start()
    {
        inventory = GetComponent<Inventory>();

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
                transform.DetachChildren();
            }
        };    
    }

    // map of pairs of items: 
    // Item 1, Item 2 -> Item 3
    [SerializeField]
    private List<CraftingRecipe> craftingRecipes;

    CraftingRecipe.Result craftings(Inventory otherItem) {
        foreach (var recipe in craftingRecipes) {
            if (recipe.Matches(inventory.InventoryItem, otherItem.InventoryItem) || recipe.Matches(otherItem.InventoryItem, inventory.InventoryItem)) {
                return recipe.getResult();
            }
        }
        return null;
    }

    public override bool tryInteract(Inventory otherItem)
    {
        var craftingResult = craftings(otherItem);

        if (craftingResult != null) {
            return true;
        }

        return (otherItem.hasItem() && inventory.isEmpty()) || (otherItem.isEmpty() && inventory.hasItem());
    }

    public override bool Interact(Inventory otherItem)
    {
        var craftingResult = craftings(otherItem);

        if (craftingResult != null) {
            Debug.Log("Crafting " + craftingResult.item + " " + craftingResult.state);
            if(craftingResult.item != null) {
                Destroy(inventory.DropItem());
                inventory.InventoryItem = Instantiate(craftingResult.item);
            }

            Debug.Log(inventory.InventoryItem);            
            if(inventory.TryGetInventoryItemComponent(out MugState state)) {
                state.state = craftingResult.state;
            } else {
                // destroy item
                Destroy(inventory.DropItem());
                // create mug with state
                var mug = Instantiate(mugPrefab);
                mug.GetComponent<MugState>().state = craftingResult.state;
                inventory.InventoryItem = mug;
            }

            Destroy(otherItem.DropItem());
            return true;
        }
        
        if (inventory.isEmpty() && otherItem.hasItem())
        {
            inventory.swapItems(otherItem);
            return true;
        }
        else if (inventory.hasItem() && otherItem.isEmpty())
        {
            inventory.swapItems(otherItem);
            return true;
        }

        return false;
    }

    void Update()
    {
        
    }
}
