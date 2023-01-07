using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    Transform characterTransform;
    [SerializeField]
    Vector3 offsetFromCharacter;
    [SerializeField]
    Vector3 cameraTargetOffset;

        
    private void Start()
    {
        characterTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        transform.position = characterTransform.position + offsetFromCharacter;
        transform.rotation = Quaternion.LookRotation(-offsetFromCharacter);
    }
}
