using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float deceleration;

    [Header("Jump")]
    public float jumpForce;
    public int maxJumps = 1;

    [Header("Ground Check")]
    //public float maxSlopeAngle = 30f;
    public float sphereRadius = 0.5f;
    public float shpereRelativeHeight = 0f;
    public float groundCheckDistance = 0.01f;
    private Vector3 groundDetectionHeight;

    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Vector2 moveInput;
    private bool jumpRequested;

    private bool isGrounded;
    private int jumpsRemaining;
    private LayerMask notPlayerLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        jumpsRemaining = maxJumps;
        groundDetectionHeight = new Vector3(0f, shpereRelativeHeight, 0f);

        notPlayerLayer = ~LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Debug.Log("Is Grounded: " + isGrounded);
        GroundCheck();
        if (isGrounded)
        {
            Move();
            if (jumpRequested)
            {
                Jump();
                jumpRequested = false;
                jumpsRemaining--;
            }
        }
        
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpRequested = true;
        }
        else if (context.canceled)
        {
            jumpRequested = false;
        }
        //isJumping = context.ReadValue<float>() > 0;
    }

    public void Move()
    {
        Vector3 targetVelocity = (moveInput.x * playerTransform.right + moveInput.y * playerTransform.forward) * moveSpeed;
        Vector3 curentVelocity = playerRigidbody.linearVelocity;
        curentVelocity.y = 0f; // Ignore y velocity
        Vector3 velocityDif = targetVelocity - curentVelocity;
        float accelRate = (targetVelocity.magnitude > 0f) ? acceleration : deceleration;

        Vector3 force = velocityDif * accelRate;
        //force.x = Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.x) + 1) * 0.5f*Mathf.Abs(velocityDif.x)) * Mathf.Sign(velocityDif.x);
        //force.z = Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.z) + 1) * 0.5f*Mathf.Abs(velocityDif.z)) * Mathf.Sign(velocityDif.z);

        playerRigidbody.AddForce(force, ForceMode.Force); //ForceMode.Force, ForceMode.Acceleration, ForceMode.VelocityChange
    }

    public void Jump()
    {
        if (playerRigidbody.linearVelocity.y < -1f)
        {
            playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, -1*Mathf.Log(Mathf.Abs(playerRigidbody.linearVelocity.y)), playerRigidbody.linearVelocity.z);
        }
        //playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void GroundCheck()
    {
        isGrounded = Physics.SphereCast(
            transform.position + groundDetectionHeight,
            sphereRadius, 
            Vector3.down, 
            out RaycastHit hitInfo, 
            sphereRadius + groundCheckDistance, 
            notPlayerLayer, 
            QueryTriggerInteraction.Ignore);
    }

/* Ground check from the tutorial
    public void GroundCheck()
    {
        Collider[] colliders;

        Vector3 spherePosition = transform.position + sphereOffset;
        colliders = Physics.OverlapSphere(spherePosition, sphereRadius, notPlayerLayer);

        if (colliders.Length == 0)
        {
            isGrounded = false;
            Debug.Log("No colliders detected.");
        }

        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                Vector3 colliderPosition = col.gameObject.transform.position;

                float groundAngle = Vector3.Angle(spherePosition, colliderPosition);

                if (groundAngle > 180f + maxSlopeAngle || groundAngle < 180f - maxSlopeAngle)
                {
                    isGrounded = true;
                }
            }
        }
    }
*/
}
