using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
    public class CharacterCamera : Player.PlayerBehaviour
    {
        Transform characterTransform;
        [SerializeField]
        Vector3 offsetFromCharacter;
        [SerializeField]
        float smoothTime = 0.1f;
        [SerializeField]
        float offsetMagnitude = 0.3f;

        Vector3 cameraOffset = Vector3.zero;
        Vector3 targetOffset;
        Vector3 currentVelocity;


        private void Start()
        {
            characterTransform = GameObject.FindWithTag("Player").transform;
        }

        void Update()
        {
            cameraOffset = Vector3.SmoothDamp(cameraOffset, -targetOffset * offsetMagnitude, ref currentVelocity, smoothTime);

            transform.position = characterTransform.position + offsetFromCharacter + cameraOffset;
            transform.rotation = Quaternion.LookRotation(-offsetFromCharacter);
        }

        public void UpdateTargetOffset(Vector3 offset) => targetOffset = offset;
    }
}