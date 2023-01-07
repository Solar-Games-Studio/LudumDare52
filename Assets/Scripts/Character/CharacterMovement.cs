using UnityEngine;
using qASIC.Input;

public class CharacterMovement : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField]
    CharacterCamera characterCamera;

    [SerializeField] InputMapItemReference i_movement;

    [SerializeField]
    float speed = 5.0f;
    [SerializeField]
    float turnSmoothTime = 0.1f;
    
    float turnSmoothVelocity;

    bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        //piss off
        var movement = i_movement.GetInputValue<Vector2>();

        Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;
        characterCamera.UpdateTargetOffset(direction);

        if (direction.magnitude > 0.1f && canMove)
        {
            characterController.Move(speed * Time.deltaTime * direction);
            float targetAngle = Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    public void LockMovement() => canMove = true;
    public void UnlockMovement() => canMove = false;
}
