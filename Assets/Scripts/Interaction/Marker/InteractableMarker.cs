using UnityEngine;
using System;

namespace Game.Interaction.Marker
{
    public class InteractableMarker : MonoBehaviour
    {
        static event Action OnChangeTarget;

        static Vector3? _target;
        public static Vector3? Target
        {
            get => _target;
            set
            {
                if (_target == value) return;

                _target = value;
                OnChangeTarget?.Invoke();
            }
        }


        [SerializeField] GameObject toggleObject;


        private void Reset()
        {
            if (transform.childCount > 0)
                toggleObject = transform.GetChild(0).gameObject;
        }

        private void Awake()
        {
            HandleTargetChange();
            OnChangeTarget += HandleTargetChange;
        }

        void HandleTargetChange()
        {
            toggleObject?.SetActive(Target != null);
        }

        private void Update()
        {
            if (Target != null)
                transform.position = Target ?? Vector3.zero;
        }
    }
}