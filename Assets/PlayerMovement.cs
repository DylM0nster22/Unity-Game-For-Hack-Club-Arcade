using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    public float lookSensitivity = 2.0f;
    public float maxLookX = 60.0f;
    public float minLookX = -60.0f;
    private float rotX;
    private CharacterController characterController;
    private Vector3 moveDirection;
    public int maxHealth = 100;
    public int health;


    [SerializeField]
    private GameObject playerCamera;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        health = maxHealth;
    }

    void Update()
    {
        Move();
        CameraLook();
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("JoystickJump")) && characterController.isGrounded)
        {
            Jump();
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y += Physics.gravity.y * Time.deltaTime;
        }
        
        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal") * speed + Input.GetAxis("JoystickHorizontal") * speed;
        float z = Input.GetAxis("Vertical") * speed + Input.GetAxis("JoystickVertical") * speed;
        Vector3 move = transform.right * x + transform.forward * z;
        moveDirection = new Vector3(move.x, moveDirection.y, move.z);
    }

    void CameraLook()
    {
        float y = Input.GetAxis("Mouse X") * lookSensitivity + Input.GetAxis("JoystickLookHorizontal") * lookSensitivity;
        rotX += Input.GetAxis("Mouse Y") * lookSensitivity + Input.GetAxis("JoystickLookVertical") * lookSensitivity;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);
        playerCamera.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);
        transform.eulerAngles += Vector3.up * y;
    }

    void Jump()
    {
        moveDirection.y = jumpForce;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Player took {damage} damage. Current health: {health}");
        
        if (health <= 0)
        {
            Die();
        }

        // Update UI if you have a health bar
        // UpdateHealthUI();
    }

    void Die()
    {
        Debug.Log("Player died!");
        // Implement player death logic here
    }

    void Respawn()
    {
        // Implement respawn logic here
        health = maxHealth;
        Debug.Log("Player respawned!");
    }

    void OnPlayerDefeated()
    {
        // Handle player defeat (e.g., play animation, disable controls)
        Debug.Log("Player defeated!");
        // You might want to respawn the player or end the game here
    }
}