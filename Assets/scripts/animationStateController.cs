using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    private bool isWalking = false;

    [SerializeField]
    private float movementThreshold = 0.01f; // Adjust this value to fine-tune sensitivity

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the player object!");
        }
        lastPosition = transform.position;
    }

    void Update()
    {
        CheckMovement();
    }

    void CheckMovement()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - lastPosition;

        // Check if there's significant movement in the X or Z axis
        bool isMoving = Mathf.Abs(movement.x) > movementThreshold || Mathf.Abs(movement.z) > movementThreshold;

        // Immediately update animation state if it changed
        if (isMoving != isWalking)
        {
            isWalking = isMoving;
            UpdateAnimationState();
        }

        lastPosition = currentPosition;
    }

    void UpdateAnimationState()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
        }
    }
}