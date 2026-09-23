using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction liftAction;
    public float moveSpeed = 0.5f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        liftAction = playerInput.actions["Lift"];
    }
    void Update()
    {
        Vector2 movement = moveAction.ReadValue<Vector2>();

        transform.Translate(movement.x * moveSpeed * Time.deltaTime, 0f, movement.y * moveSpeed * Time.deltaTime);

        Vector2 lifting = liftAction.ReadValue<Vector2>();
        transform.Translate(0f, lifting.y * moveSpeed * Time.deltaTime, 0f);
    }
}
