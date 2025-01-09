using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CoffeMachine : Interactable
{
    public SphereCollider coffeMachineInteractionDistance;
    public GameObject brewdCoffePrefab;

    private Inventory inventory;
    private Transform slot;


    private Animator animator;

    [SerializeField]
    public enum CoffeMachineState
    {
        Empty,
        CoffeInside,
        Brewed,
    }

    public CoffeMachineState state;

    private float brewTime = 5f;
    public float brewTimer = 0f;

    public override bool Interact(Inventory item)
    {
        if (inventory.InventoryItem == null && item.InventoryItem != null)
        {
            inventory.swapItems(item);
            UpdateState();
            return true;
        }
        else if (inventory.InventoryItem != null && item.InventoryItem == null)
        {
            inventory.swapItems(item);
            UpdateState();
            return true;
        }

        return false;
    }

    void resetAnimatorStates() {
        animator.SetBool("Alert", false);
        animator.SetBool("CoffeReady", false);
        animator.SetBool("MachineFull", false);
        animator.SetBool("CoffeBrewing", false);
    }

    void UpdateState() {
        resetAnimatorStates();
        
        // Interrupt brewing if necessary
        if (brewTimer > 0) {
            brewTimer = 0;
        }

        if(state == CoffeMachineState.Brewed) {
            animator.SetBool("MachineFull", true);
            return;
        }
        

        if (inventory.InventoryItem == null) {
            animator.SetBool("Alert", true);
            state = CoffeMachineState.Empty;
            return;
        }


        var coffeSeeds = inventory.GetInventoryItemComponent<CoffeSeeds>();
        if (coffeSeeds != null) {
            state = CoffeMachineState.CoffeInside;
            Destroy(inventory.DropItem());
            return;
        }
        if (state != CoffeMachineState.CoffeInside) {
            animator.SetBool("Alert", true);
        }

        // var mugState = inventory.GetInventoryItemComponent<MugState>();
        // if (mugState != null) {
        //     state = CoffeMachineState.Empty;
        //     return;
        // }
    }

    public override bool tryInteract(Inventory item) {
        return true;
    }

    void Run(Inventory item) {
        if (state == CoffeMachineState.CoffeInside && item.GetInventoryItemComponent<MugState>() != null) {
            brewTimer = brewTime;
            state = CoffeMachineState.Brewed;
        } else if (state == CoffeMachineState.Brewed && item.InventoryItem == null) {
            var newItem = Instantiate(brewdCoffePrefab);
            item.InventoryItem = newItem;
            state = CoffeMachineState.Empty;
        } else if (state == CoffeMachineState.Empty) {
            // Additional logic for empty state if needed
        }
    }

    // Run 
    public override bool Use()
    {
        Run(inventory);
        return false;
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

        animator = GetComponent<Animator>();


        Debug.Assert(inventory != null);
        Debug.Assert(slot != null);
        Debug.Assert(animator != null);
        Debug.Assert(brewdCoffePrefab != null);

        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        if (brewTimer > 0)
        {
            animator.SetBool("CoffeBrewing", true);
            brewTimer -= Time.deltaTime;
            if (brewTimer <= 0)
            {
                // set item state to brewed
                state = CoffeMachineState.Brewed;

                if (inventory.TryGetInventoryItemComponent<MugState>(out var mugState))
                {
                    switch (mugState.state)
                    {
                        case MugState.State.Empty:
                            mugState.state = MugState.State.Expresso;
                            break;
                        case MugState.State.Expresso:
                            mugState.state = MugState.State.DoubleExpresso;
                            break;
                        // Add more cases if there are more states
                    }
                }
            }
        } else {
            animator.SetBool("CoffeBrewing", false);
        }
    }
}
