using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Expects an action named "Move" in the PlayerInput Actions asset.
        moveAction = playerInput.actions != null ? playerInput.actions.FindAction("Move") : null;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
            return;
        }

        Debug.LogWarning("PlayerMovement could not find a 'Move' action on PlayerInput.");
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
    }

    private void FixedUpdate()
    {
        if (moveAction == null)
        {
            return;
        }

        moveInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = moveSpeed * Time.fixedDeltaTime * new Vector3(moveInput.x, 0f, moveInput.y);
        rb.MovePosition(rb.position + movement);
    }
}
