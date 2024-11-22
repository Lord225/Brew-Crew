using UnityEngine;
using UnityEngine.AI;

public class NavMeshPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Maximum movement speed of the player
    public float acceleration = 10f; // How quickly the player speeds up
    public float deceleration = 10f; // How quickly the player slows down
    public float turnSpeed = 720f; // Rotation speed in degrees per second

    private Vector3 accelerationDirection;

    void Update()
    {
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
            transform.position = targetPosition;

            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        } else {
            // find the closest valid position
            if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
            {
                // set acceleratrion in collision direction to 0
                accelerationDirection = Vector3.zero;

                // Move the player to the valid position
                transform.position = hit.position;

                // Smoothly rotate the player to face the movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
    }
}
