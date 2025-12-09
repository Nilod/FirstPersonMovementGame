using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform playerTransform;
    public float sensitivity;
    private InputAction lookAction;
    private Vector2 lookValue;
    private float xRotation;
    private float yRotation;

    void Start()
    {
        lookAction = GetComponentInParent<PlayerInput>().actions["Look"];
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        lookValue = lookAction.ReadValue<Vector2>();
        Debug.Log($"{lookValue}");

        xRotation -= lookValue.y * sensitivity * Time.deltaTime;
        yRotation += lookValue.x * sensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerTransform.rotation = Quaternion.Euler(0, yRotation, 0);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
