using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    Transform sellPoint;
    Transform targetPoint;
    Transform exitPoint;

    [SerializeField]
    Animator modelAnimator;

    float betweenDistance = 0.2f;
    [SerializeField]
    float speed = 2.0f;
    bool isMoving = true;
    bool hasArrived = false;

    public UnityEvent onNewCustomerArrived;

    private void FixedUpdate()
    {
        if (Vector3.Distance(targetPoint.position, transform.position) > betweenDistance)
        {
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            transform.position += speed * Time.fixedDeltaTime * direction;
            transform.rotation = Quaternion.LookRotation(direction);
            isMoving = true;
        }
        else
        {
            isMoving = false;
            if (targetPoint == exitPoint)
                Destroy(gameObject);
        }

        if (targetPoint == sellPoint)
        {
            betweenDistance = 0.1f;
            if (!hasArrived)
            {
                onNewCustomerArrived.Invoke();
                transform.Rotate(new Vector3(0, -90, 0));
                hasArrived = true;
            }
        }
    }

    private void Update()
    {
        modelAnimator.SetFloat("Blend", isMoving ? 1 : 0);
    }

    public void GoAway()
    {
        targetPoint = exitPoint;
    }

    public void SetSpeed(float speed) => this.speed = speed;
    public void SetBetweenDistance(float betweenDistance) => this.betweenDistance = betweenDistance;
    public void SetSellPoint(Transform sellPoint) => this.sellPoint = sellPoint;
    public void SetExitPoint(Transform exitPoint) => this.exitPoint = exitPoint;
    public void SetTarget(Transform targetPoint) => this.targetPoint = targetPoint;
}
