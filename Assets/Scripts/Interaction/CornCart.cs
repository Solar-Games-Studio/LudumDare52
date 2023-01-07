using UnityEngine;
using UnityEngine.Events;

namespace Game.Interaction
{
    public class CornCart : MonoBehaviour, IInteractable
    {
        [SerializeField]
        GameObject mainUI;
        [SerializeField]
        float cartRange;

        Transform playerTransform;
        bool hasInteracted = false;

        public void Start()
        {
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        public void FixedUpdate()
        {
            if (hasInteracted && Vector3.Distance(transform.position, playerTransform.position) > cartRange)
            {   
                hasInteracted = false;
                SetCartEnabled(false);
            }
        }

        public void Interact()
        {
            hasInteracted = !hasInteracted;
            SetCartEnabled(hasInteracted);
        }

        void SetCartEnabled(bool enabled)
        {
            mainUI.SetActive(enabled);
        }
    }
}
