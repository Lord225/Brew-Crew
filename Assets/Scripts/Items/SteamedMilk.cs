using System;
using UnityEngine;

public class SteamedMilk : PickableItem
{ 

    public GameObject spilledMilkObject;

    void Start()
    {
        
    }

    private int flippedFrames = 0;

    void FixedUpdate()
    {
        if (Math.Abs(Vector3.Dot(transform.up, Vector3.down)) > 0.5 || Vector3.Dot(transform.right, Vector3.down) > 0.5)
        {
            flippedFrames++;
            if (flippedFrames > 3)
            {
                // instantiate spilled milk object on ground and destroy this object
                RaycastHit hit;

                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    Instantiate(spilledMilkObject, hit.point, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
        else
        {
            flippedFrames = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
