using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveValue;
    private bool jumpPressed;

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
        Vector3 moveDirection = moveValue.x * playerTransform.right + moveValue.y * playerTransform.forward;
        playerRigidbody.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
    }
}
