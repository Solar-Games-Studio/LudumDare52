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

        public event Action<bool> OnChangeState;

        public void ChangeState(bool show)
        {
            OnChangeState?.Invoke(show);
        }
    }
}