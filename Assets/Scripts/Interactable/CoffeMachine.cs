using UnityEngine;

public class CoffeMachine : MonoBehaviour
{
    public SphereCollider coffeMachineInteractionDistance;
    

    void Start()
    {
        coffeMachineInteractionDistance = GetComponent<SphereCollider>();
        

        
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
