using UnityEngine;

namespace Game.Character
{
    public class CharacterChangeSpeed : MonoBehaviour
    {
        [SerializeField] string modifierName;
        [SerializeField] float trueValue = 0f;
        [SerializeField] float falseValue = 1f;

        public void ChangeSpeed(bool state) =>
            ChangeSpeed(state ? trueValue : falseValue);

        public void ChangeSpeed(float value) =>
            CharacterMovement.ChangeMultiplier(modifierName, value);
    }
}