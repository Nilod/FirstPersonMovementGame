using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float deceleration;
    public float jumpForce;
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private bool isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = (moveInput.x * playerTransform.right + moveInput.y * playerTransform.forward) * moveSpeed;
        Vector3 curentVelocity = playerRigidbody.linearVelocity;
        curentVelocity.y = 0f; // Ignore y velocity
        Vector3 velocityDif = targetVelocity - curentVelocity;
        float accelRate = (targetVelocity.magnitude > 0f) ? acceleration : deceleration;

        Vector3 force = velocityDif * accelRate;
        //force.x = Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.x) + 1) * 0.5f*Mathf.Abs(velocityDif.x)) * Mathf.Sign(velocityDif.x);
        //force.z = Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.z) + 1) * 0.5f*Mathf.Abs(velocityDif.z)) * Mathf.Sign(velocityDif.z);

        playerRigidbody.AddForce(force, ForceMode.Force); // Add velocity to current velocity

        if (isJumping && IsGrounded())
        {
            if (playerRigidbody.linearVelocity.y < -1f)
            {
                playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, -1*Mathf.Log(Mathf.Abs(playerRigidbody.linearVelocity.y)), playerRigidbody.linearVelocity.z);
            }
            //playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.ReadValue<float>() > 0;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(playerTransform.position, Vector3.down, 1.1f);
    }
}
