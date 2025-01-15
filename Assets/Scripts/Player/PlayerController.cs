using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Maximum movement speed of the player
    public float acceleration = 10f; // How quickly the player speeds up
    public float deceleration = 10f; // How quickly the player slows down
    public float turnSpeed = 720f; // Rotation speed in degrees per second


    public AudioClip pickupSound;
    public AudioClip dropSound;
    public AudioClip useSound;

    public AudioClip throwSound;

    private Vector3 accelerationDirection;

    private Vector3 facingDirection;

    private Inventory inventory;
    private Transform hand;

    private AudioSource audio;
    
    void Start()
    {
        inventory = GetComponent<Inventory>();
        hand = transform.Find("Hand");
        audio = GetComponent<AudioSource>();

        inventory.OnInventoryItemChanged += (GameObject newItem) =>
        {
            if (newItem != null)
            {
                newItem.transform.SetParent(hand);
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
                hand.DetachChildren();
            }
        };


        Debug.Assert(inventory != null, "Inventory component is missing");
        Debug.Assert(hand != null, "Hand transform is missing");
        Debug.Assert(audio != null, "Audio source is missing");
    }



    void Movment() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        accelerationDirection += moveDirection * acceleration * Time.deltaTime;

        if (moveDirection.magnitude < 0.1f)
        {
            accelerationDirection = Vector3.MoveTowards(accelerationDirection, Vector3.zero, deceleration * Time.deltaTime);
        }

        accelerationDirection = Vector3.ClampMagnitude(accelerationDirection, moveSpeed);

        Vector3 targetPosition = transform.position + accelerationDirection * Time.deltaTime;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
        {
            // Move the player to the valid position
            transform.position = hit.position;
        }
        else
        {
            // find the closest valid position
            if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
            {
                // set acceleration in collision direction to 0
                accelerationDirection = Vector3.zero;

                // Move the player to the valid position
                transform.position = hit.position;

                // Smoothly rotate the player to face the movement direction if there is any movement
                if (moveDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                }
            }
        }

        facingDirection = Vector3.Lerp(facingDirection, moveDirection, 0.1f);

        // Smoothly rotate the player to face the movement direction if there is any movement
        if (facingDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(facingDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
    private float lastThrowTime;
    private float cooldownTime = 0.5f; // 10 frames at 20 FPS

    void ItemThrow() {
        if (Time.time - lastThrowTime < cooldownTime) return;

        if(inventory.InventoryItem != null && Input.GetKeyDown(KeyCode.E)) {
            GameObject item = inventory.DropItem();
            if (item.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
            }
            lastThrowTime = Time.time;

            audio.PlayOneShot(throwSound);
        }
    }

    void InteractWithObjects() {
        if (Time.time - lastThrowTime < cooldownTime) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);

        Interactable[] machines = colliders.Select(collider => collider.GetComponent<Interactable>()).Where(coffeMachine => coffeMachine != null).ToArray();

        PickableItem[] items = colliders.Select(collider => collider.GetComponent<PickableItem>()).Where(item => item != null && item.owner == null).ToArray();

        if(items.Length > 0 && Input.GetKeyDown(KeyCode.E) && inventory.InventoryItem == null) {
            var closestItem = items.OrderBy(item => Vector3.Distance(transform.position, item.transform.position)).First();
            inventory.InventoryItem = closestItem.gameObject;
            audio.PlayOneShot(pickupSound);
            return;
        }

        // get closest machine
        var closestMachine = machines.Where(machine => machine.tryInteract(inventory)).OrderBy(machine => Vector3.Distance(transform.position, machine.transform.position)).FirstOrDefault();

        if (closestMachine != null && Input.GetKeyDown(KeyCode.E))
        {
            closestMachine.Interact(inventory);
            audio.PlayOneShot(useSound);
        } else {
            ItemThrow();
        }

        if (closestMachine != null && Input.GetKeyDown(KeyCode.Space))
        {
            closestMachine.Use();
            audio.PlayOneShot(useSound);
        }

    }

    void Update()
    {
        Movment();
        InteractWithObjects();
    }
}
