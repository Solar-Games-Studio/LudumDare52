using UnityEngine;
using UnityEngine.Events;

namespace Game.NPCs
{
    public class NPC : MonoBehaviour
    {
        Transform sellPoint;
        Transform targetPoint;
        Transform exitPoint;

        [SerializeField]
        Animator modelAnimator;
        [SerializeField]
        Transform bubblePosition;
        [SerializeField]
        float speed = 2.0f;

        float betweenDistance = 0.2f;
        bool isMoving = true;
        bool hasArrived = false;

        public UnityEvent OnNewCustomerArrived;
        public UnityEvent<NPC> OnExit;

        private void Start()
        {
            BubbleFactory.ShowBubbleOnTransform(BubbleType.Big, bubblePosition);
        }

        private void FixedUpdate()
        {
            if (targetPoint == null)
                targetPoint = exitPoint;
                 
            isMoving = Vector3.Distance(targetPoint.position, transform.position) > betweenDistance;
            
            if (isMoving)
            {
                var direction = (targetPoint.position - transform.position).normalized;
                transform.position += speed * Time.fixedDeltaTime * direction;
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else if (targetPoint == exitPoint)
            {
                OnExit.Invoke(this);
                Destroy(gameObject);
            }

            if (targetPoint == sellPoint)
            {
                betweenDistance = 0.1f;
                if (!hasArrived)
                {
                    OnNewCustomerArrived.Invoke();
                    transform.Rotate(new Vector3(0, -90, 0));
                    hasArrived = true;
                }
            }
        }

        private void Update()
        {
            modelAnimator?.SetFloat("Blend", isMoving ? 1 : 0);
        }

        public void GoAway()
        {
            targetPoint = exitPoint;
        }

        internal void SetSpeed(float speed) => this.speed = speed;
        internal void SetBetweenDistance(float betweenDistance) => this.betweenDistance = betweenDistance;
        internal void SetSellPoint(Transform sellPoint) => this.sellPoint = sellPoint;
        internal void SetExitPoint(Transform exitPoint) => this.exitPoint = exitPoint;
        internal void SetTarget(Transform targetPoint) => this.targetPoint = targetPoint;

        public void DisplayDialogue(string text, float time)
        {
            Debug.LogError("Jakubie to te? napraw");
        }
    }
}