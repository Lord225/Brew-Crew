using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    public GameObject clientPrefab;
    private Animator animator;
    public Transform spawn;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spawn = transform.Find("Spawn");
    }

    public ClientScript SpawnClient()
    {
        if (clientPrefab != null)
        {
            var client = Instantiate(clientPrefab, spawn.position, spawn.rotation);
            PlayAnimation();

            return client.GetComponent<ClientScript>();
        }

        return null;
    }

    private void PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Spawn");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Client"))
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetTrigger("Spawn");
        }
    }
}
