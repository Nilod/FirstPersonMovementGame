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

    [Header("Surface Checking")]
    public float maxSlopeAngle = 30f;

    private Transform playerTransform;
    private Rigidbody playerRigidbody;

    /*
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    */
    private Vector2 moveInput;
    private bool requestJump;

    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        /*
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Move();
        if (requestJump && isGrounded)
        {
            Jump();
            requestJump = false;
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
            requestJump = true;
        }
        else if (context.canceled)
        {
            requestJump = false;
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

        playerRigidbody.AddForce(force, ForceMode.Acceleration); //ForceMode.Force, ForceMode.Acceleration, ForceMode.VelocityChange
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

/*
    public bool IsGrounded()
    {
        //return Physics.SphereCast(playerTransform.position, groundDetectionSphereRadius, Vector3.down, out RaycastHit hitInfo, 1.1f, QueryTriggerInteraction.Ignore);
    }
*/

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle <= maxSlopeAngle)
            {
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle <= maxSlopeAngle)
            {
                isGrounded = false;
            }
        }
    }
}
