using UnityEngine;
using System.Collections.Generic;
using System;

namespace Game.Player
{
    public class PlayerReference : MonoBehaviour
    {
        public static PlayerReference Singleton;

        Dictionary<Type, PlayerBehaviour> _behaviours = new Dictionary<Type, PlayerBehaviour>();

        private void Awake()
        {
            Singleton = this;

            var behaviours = GetComponents<PlayerBehaviour>();
            foreach (var item in behaviours)
            {
                item.playerReference = this;
                var type = item.GetType();
                if (_behaviours.ContainsKey(type)) continue;
                _behaviours.Add(type, item);
            }
        }

        public T GetBehaviour<T>() where T : PlayerBehaviour =>
            (T)GetBehaviour(typeof(T));

        public PlayerBehaviour GetBehaviour(Type type) =>
            _behaviours.TryGetValue(type, out PlayerBehaviour behaviour) ? behaviour : null;
    }
}