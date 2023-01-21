using UnityEngine;
using qASIC.Input;
using System;

namespace Game.Prompts
{
    [CreateAssetMenu(fileName = "New Prompt", menuName = "Scriptable Objects/Prompts/Prompt")]
    public class Prompt : ScriptableObject
    {
        public bool axisSprite;
        public InputMapItemReference item;
        public string text;

        [Space]
        [Tooltip("If this value is higher than the rest, other items will be hidden untill this one will be set to false")]
        public int overrideLayer; 

        public event Action<bool> OnChangeState;

        public void ChangeState(bool show)
        {
            OnChangeState?.Invoke(show);
        }
    }
}