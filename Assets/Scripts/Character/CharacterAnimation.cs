using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{


    public class CharacterAnimation : MonoBehaviour
    { 
        [SerializeField]
        Animator modelAnimator;
        [SerializeField]
        float animationTransitionTime = 0.1f;
        
        bool isMoving;
        bool isHolding;
        bool isHarvesting;

        public void Update()
        {
            modelAnimator.SetBool("IsHarvesting", isHarvesting);
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
        public void SetHarvestingState(bool state)
        {
            isHarvesting = state;
        }

    }
}
