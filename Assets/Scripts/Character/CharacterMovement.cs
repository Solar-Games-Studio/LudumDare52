using UnityEngine;
using qASIC.Input;
using System.Collections.Generic;

namespace Game.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        CharacterController characterController;
        [SerializeField]
        Animator modelAnimator;

        [SerializeField]
        CharacterCamera characterCamera;

        [SerializeField] InputMapItemReference i_movement;

        [SerializeField]
        float speed = 5.0f;
        [SerializeField]
        float turnSmoothTime = 0.1f;

        [SerializeField]
        float animationTransitionTime = 0.3f;

        static Dictionary<string, float> _speedMultipliers = new Dictionary<string, float>();
        public static float FinalSpeedMultiplier { get; private set; } = 1f;


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

            var direction = new Vector3(movement.x, 0, movement.y).normalized;
            characterCamera.UpdateTargetOffset(FinalSpeedMultiplier == 0f || !canMove ? Vector3.zero : direction);

            modelAnimator.SetFloat("Speed", (direction.magnitude > 0.1f ? 1.0f : 0.0f) * FinalSpeedMultiplier, animationTransitionTime, Time.deltaTime);

            if (direction.magnitude > 0.1f && canMove)
            {
                characterController.Move(speed * Time.deltaTime * direction * FinalSpeedMultiplier);
                float targetAngle = Mathf.Atan2(-direction.x, -direction.z) * Mathf.Rad2Deg;
                float angle = FinalSpeedMultiplier == 0f ? 
                    transform.eulerAngles.y : 
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime / FinalSpeedMultiplier);

                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        public void LockMovement() => canMove = true;
        public void UnlockMovement() => canMove = false;

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