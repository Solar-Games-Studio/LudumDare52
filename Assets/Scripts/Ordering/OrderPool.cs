using UnityEngine;

namespace Game.Ordering
{
    [CreateAssetMenu(fileName = "New Order Pool", menuName = "Scriptable Objects/Ordering/Order Pool")]
    public class OrderPool : ScriptableObject
    {
        [Min(1)] public int orderAmount;

        [Label("Orders")]
        public Order[] orders;
        [Tooltip("These orders will always appear only 1 time")] public Order[] oneTimeOrders;
    }
}