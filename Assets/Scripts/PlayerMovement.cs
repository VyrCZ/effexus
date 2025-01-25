using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public bool canJump = true;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool touchingCeil;

    public Transform groundCheck;
    public Transform ceilCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool isHovering = false;

    public float teleportSpeed = 10f;

    private Coroutine teleportCoroutine;
    private Rigidbody rb;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        touchingCeil = Physics.CheckSphere(ceilCheck.position, groundDistance, groundMask);


        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(moveSpeed * Time.deltaTime * move);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isHovering){
            velocity.y = Mathf.Clamp(velocity.y, 0, Mathf.Infinity);
        }

        if (touchingCeil){
            velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, 0);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void TeleportPlayer(Vector3 pos)
    {
        // Stop any ongoing teleport to avoid overlap
        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
        }

        // Start the smooth teleport coroutine
        teleportCoroutine = StartCoroutine(SmoothTeleport(pos, teleportSpeed));
    }

    private IEnumerator SmoothTeleport(Vector3 targetPos, float speed)
    {
        // Disable the Rigidbody to prevent physics interference
        if (rb != null)
        {
            rb.isKinematic = true;
            GetComponent<CharacterController>().enabled = false;
        }

        // Smoothly move the player
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            Physics.SyncTransforms();
            yield return null;
        }

        // Ensure the final position is set
        transform.position = targetPos;

        // Re-enable the Rigidbody
        if (rb != null)
        {
            rb.isKinematic = false;
            GetComponent<CharacterController>().enabled = true;
        }
        Physics.SyncTransforms();

        teleportCoroutine = null; // Clear the reference when done
    }
}
