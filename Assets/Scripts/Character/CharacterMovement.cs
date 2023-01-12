using UnityEngine;
using qASIC.Input;
using System.Collections.Generic;

namespace Game.Character
{
    public class CharacterMovement : Player.PlayerBehaviour
    {
        CharacterController characterController;

        [SerializeField]
        CharacterAnimation characterAnimation;

        [SerializeField]
        CharacterCamera characterCamera;

        [SerializeField] InputMapItemReference i_movement;

        [SerializeField]
        float speed = 5.0f;
        [SerializeField]
        float turnSmoothTime = 0.1f;
        [SerializeField]
        float gravity = 2.0f;

        static Dictionary<string, float> _speedMultipliers = new Dictionary<string, float>();
        public static float FinalSpeedMultiplier { get; private set; } = 1f;

        public Vector3 Direction { get; private set; }


        float turnSmoothVelocity;

        bool canMove = true;

        void Start()
        {
            ResetMultiplier();
            characterController = GetComponent<CharacterController>();
        }


        void Update()
        {
            //piss off
            var movement = i_movement.GetInputValue<Vector2>();

            Direction = new Vector3(movement.x, 0, movement.y).normalized;
            characterCamera.UpdateTargetOffset(FinalSpeedMultiplier == 0f || !canMove ? Vector3.zero : Direction);

            if (!characterController.isGrounded)
                characterController.Move(gravity * Time.deltaTime * Vector3.down);
            characterAnimation?.SetMovingState(false);

            if (Direction.magnitude > 0.1f && canMove)
            {
                characterController.Move(speed * Time.deltaTime * Direction * FinalSpeedMultiplier);
                float targetAngle = Mathf.Atan2(-Direction.x, -Direction.z) * Mathf.Rad2Deg;
                float angle = FinalSpeedMultiplier == 0f ? 
                    transform.eulerAngles.y : 
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime / FinalSpeedMultiplier);

                characterAnimation?.SetMovingState(true);
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        public void LockMovement() => canMove = false;
        public void UnlockMovement() => canMove = true;

        public static void ChangeMultiplier(string indentifier, float value)
        {
            if (_speedMultipliers.ContainsKey(indentifier))
            {
                _speedMultipliers[indentifier] = value;

                FinalSpeedMultiplier = 1f;
                foreach (var item in _speedMultipliers)
                    FinalSpeedMultiplier *= item.Value;

                return;
            }

            _speedMultipliers.Add(indentifier, value);
            FinalSpeedMultiplier *= value;
        }

        public static void ResetMultiplier()
        {
            _speedMultipliers.Clear();
            FinalSpeedMultiplier = 1f;
        }
    }
}