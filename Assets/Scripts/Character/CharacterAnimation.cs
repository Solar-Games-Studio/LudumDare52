using UnityEngine;

namespace Game.Character
{
    public class CharacterAnimation : Player.PlayerBehaviour
    { 
        [SerializeField]
        Animator modelAnimator;
        [SerializeField]
        float animationTransitionTime = 0.1f;
        
        bool isMoving;
        bool isHolding;

        public void Update()
        {
            modelAnimator.SetBool("IsHolding", isHolding);
            modelAnimator.SetFloat("Blend", isMoving ? 1.0f : 0.0f, animationTransitionTime, Time.deltaTime);
        }
        
        public void SetMovingState(bool state)
        {
            isMoving = state;
        }
        public void SetHoldingState(bool state)
        {
            isHolding = state;
        }
        public void Harvest()
        {
            modelAnimator.SetTrigger("Harvest");
        }

    }
}
