using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float maxSpeedChange;
    public float acceleration;
    public float deceleration;

    [Header("Air Movement")]
    public float airAcceleration;
    public float airDeceleration;

    [Header("Jump")]
    public float jumpForce;
    public int maxAirJumps = 1;

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
    private int airJumpsRemaining;
    private LayerMask notPlayerLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        airJumpsRemaining = maxAirJumps;
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
        Debug.Log("Velocity: " + Vector3.Magnitude(playerRigidbody.linearVelocity));
        GroundCheck();
        if (isGrounded)
        {
            airJumpsRemaining = maxAirJumps;
            GroundMove();
            if (jumpRequested)
            {
                GroundJump();
                jumpRequested = false;
            }
        }
        else
        {
            AirMove();
            if (jumpRequested && airJumpsRemaining > 0)
            {
                AirJump();
                jumpRequested = false;
                airJumpsRemaining--;
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

    public void GroundMove()
    {
        Vector3 targetVelocity = new Vector3(moveInput.x, 0f,  moveInput.y) * moveSpeed;
        targetVelocity = playerTransform.TransformDirection(targetVelocity);
        Vector3 curentVelocity = playerRigidbody.linearVelocity;
        curentVelocity.y = 0f; // Ignore y velocity
        Vector3 velocityDif = targetVelocity - curentVelocity;

        float accelRate = (Vector3.Dot(curentVelocity, targetVelocity) > 0f) ? acceleration : deceleration; // 90° 

        Vector3 force = new Vector3();
        force.x = Mathf.Log(Mathf.Abs(velocityDif.x) + 1) * accelRate * Mathf.Sign(velocityDif.x);
        force.z = Mathf.Log(Mathf.Abs(velocityDif.z) + 1) * accelRate * Mathf.Sign(velocityDif.z);
        // sqrt(log(x+1)*a*x)

        Vector3.ClampMagnitude(force, maxSpeedChange);

        playerRigidbody.AddForce(force, ForceMode.VelocityChange); //ForceMode.Force, ForceMode.Acceleration, ForceMode.VelocityChange
    }

    public void AirMove()
    {
        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        targetVelocity = playerTransform.TransformDirection(targetVelocity);
        Vector3 curentVelocity = playerRigidbody.linearVelocity;
        curentVelocity.y = 0f; // Ignore y velocity
        Vector3 velocityDif = targetVelocity - curentVelocity;

        float accelRate = (Vector3.Dot(curentVelocity, targetVelocity) > 0f) ? airAcceleration : airDeceleration; // 90° 

        Vector3 force = new Vector3();
        force.x = Mathf.Log(Mathf.Abs(velocityDif.x) + 1) * accelRate * Mathf.Sign(velocityDif.x);
        force.z = Mathf.Log(Mathf.Abs(velocityDif.z) + 1) * accelRate * Mathf.Sign(velocityDif.z);
        // sqrt(log(x+1)*a*x)

        Vector3.ClampMagnitude(force, maxSpeedChange);

        playerRigidbody.AddForce(force, ForceMode.VelocityChange); //ForceMode.Force, ForceMode.Acceleration, ForceMode.VelocityChange
    }

    public void GroundJump()
    {
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z); 
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void AirJump()
    {
        if (playerRigidbody.linearVelocity.y < -1f)
        {
            playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, -1*Mathf.Log(Mathf.Abs(playerRigidbody.linearVelocity.y)), playerRigidbody.linearVelocity.z);
        }
        else
        {
            playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);
        }

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
}
