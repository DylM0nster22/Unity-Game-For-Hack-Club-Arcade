using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    public float walkSpeed = 5f; // Default walk speed
    public float sprintSpeed = 10f; // Default sprint speed

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Crouching")]
    public float crouchSpeed = 2.5f; // Default crouch speed
    public float crouchYScale = 0.5f;
    private float startYScale;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    public static event Action OnPlayerDeath;

    private PlayerStatusUI playerHUD;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    [Header("Camera")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;
    private float xRotation = 0f;

    private EnemyController[] enemies;

    [Header("Weapon Management")]
    public WeaponManager weaponManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    private float footstepTimer = 0f;
    public float footstepInterval = 0.5f;
    private AudioSource continuousAudioSource; // New audio source for continuous sounds

    [Header("Game Management")]
    public GameController gameController; // Added GameController reference

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        // Initialize health
        currentHealth = maxHealth;

        // Find the PlayerStatusUI script
        playerHUD = FindObjectOfType<PlayerStatusUI>();
        if (playerHUD != null)
        {
            playerHUD.SetHealth(currentHealth);
        }

        startYScale = transform.localScale.y;

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Find all enemies in the scene
        enemies = FindObjectsOfType<EnemyController>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Create a new AudioSource for continuous sounds
        continuousAudioSource = gameObject.AddComponent<AudioSource>();
        continuousAudioSource.loop = true;
        continuousAudioSource.playOnAwake = false;
    }

    private void Update()
    {
        // Check if the game is paused
        if (Time.timeScale == 0f)
            return;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        LookAround();

        PlayMovementSounds();

        // handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        // Check if the game is paused
        if (Time.timeScale == 0f)
            return;

        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Debug logs to check input
        Debug.Log($"Horizontal Input: {horizontalInput}, Vertical Input: {verticalInput}");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        // Debug log to check movement state
        Debug.Log($"Movement State: {state}, Move Speed: {moveSpeed}");
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Debug log to check move direction
        Debug.Log($"Move Direction: {moveDirection}");

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    public void Respawn()
    {
        Debug.Log("Player respawned.");
        // Reset health
        currentHealth = maxHealth;

        // Update the HUD
        if (playerHUD != null)
        {
            playerHUD.SetHealth(currentHealth);
        }

        // Reset the wave spawner
        if (gameController != null)
        {
            Debug.Log("Calling RespawnAllEntities.");
            gameController.RespawnAllEntities(); // This will reset the enemies
        }

        // Reset the player's position or any other respawn logic if needed
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (playerHUD != null)
        {
            playerHUD.SetHealth(currentHealth);
        }

        if (currentHealth == 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (playerHUD != null)
        {
            playerHUD.SetHealth(currentHealth);
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void PlayMovementSounds()
    {
        bool isMoving = grounded && (horizontalInput != 0 || verticalInput != 0);

        if (isMoving)
        {
            AudioClip clipToPlay = (state == MovementState.sprinting) ? runSound : walkSound;

            if (clipToPlay != null && !continuousAudioSource.isPlaying)
            {
                continuousAudioSource.clip = clipToPlay;
                continuousAudioSource.Play();
            }
            else if (clipToPlay != continuousAudioSource.clip)
            {
                continuousAudioSource.Stop();
                continuousAudioSource.clip = clipToPlay;
                continuousAudioSource.Play();
            }
        }
        else
        {
            if (continuousAudioSource.isPlaying)
            {
                continuousAudioSource.Stop();
            }
        }
    }
}

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 20f;

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // Ensure the projectile does not collide with the player
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

        // Apply force to the projectile
        projectileRb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
    }
}