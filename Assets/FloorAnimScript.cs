using UnityEngine;

public class FloorAnimScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animation anim;


    void Start()
    {
        anim = GetComponent<Animation>();       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if (other.gameObject.CompareTag("Player"))
        {
            anim.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
