using System;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "New UI Toggle Callback", menuName = "Scriptable Objects/UI/Toggle Callback")]
    public class UIToggleCallback : ScriptableObject
    {
        public Action<object, bool> OnToggle;
    }
}