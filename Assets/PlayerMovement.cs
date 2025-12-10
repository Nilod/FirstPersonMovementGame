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
    private Vector2 moveValue;

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
        moveValue = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Move();
        if (jumpAction.triggered)
        {
            Jump();
        }
    }

    void Move()
    {
        //Vector3 moveDirection = moveValue.x * playerTransform.right + moveValue.y * playerTransform.forward;
        //playerRigidbody.AddForce(moveDirection * moveSpeed, ForceMode.Force);

        Vector3 targetVelocity = (moveValue.x * playerTransform.right + moveValue.y * playerTransform.forward) * moveSpeed;
        Vector3 velocityDif = targetVelocity - playerRigidbody.linearVelocity;
        velocityDif.y = 0; // Prevents affecting vertical velocity
        float accelRate = (targetVelocity.magnitude > 0.1f) ? acceleration : deceleration;
        /*
        Vector3 force = new Vector3(Mathf.Pow(Mathf.Abs(velocityDif.x) * accelRate, 2) * Mathf.Sign(velocityDif.x),
                                    0,
                                    Mathf.Pow(Mathf.Abs(velocityDif.z) * accelRate, 2) * Mathf.Sign(velocityDif.z));
        */
        Vector3 force = Vector3.zero;
        if (Mathf.Abs(velocityDif.x) > 0)
        {
            force.x = velocityDif.x * 0.2f;
            //force.x = Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.x) + 1) * Mathf.Abs(velocityDif.x)) * Mathf.Sign(velocityDif.x);
        }
        if (Mathf.Abs(velocityDif.z) > 0)
        {
            force.z = velocityDif.z * 0.2f;
            //force.z = Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.z) + 1) * Mathf.Abs(velocityDif.z)) * Mathf.Sign(velocityDif.z);
        }

        Debug.Log("Force.x: " + force.x + " Force.z: " + force.z);
        /*
        Vector3 force = new Vector3(Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.x)) * velocityDif) * Mathf.Sign(velocityDif.x),
                                    0,
                                    Mathf.Sqrt(Mathf.Log(Mathf.Abs(velocityDif.z)) * velocityDif.z) * Mathf.Sign(velocityDif.z));
    */
        playerRigidbody.AddForce(force, ForceMode.VelocityChange); // Add velocity to current velocity
    }

    void Jump()
    {
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
