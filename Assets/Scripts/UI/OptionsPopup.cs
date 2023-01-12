using qASIC.SettingsSystem;
using qASIC.SettingsSystem.Menu;
using UnityEngine;
using System;
using System.Linq;

namespace Game.UI
{
    public class OptionsPopup : MenuOption
    {
        public string[] values = new string[] { "false", "true", };
        protected int index;

        protected const char valueSplit = '|';

        public void Next()
        {
            index++;

            if (index >= values.Length)
                index = 0;

            UpdateSetting();
        }

        public void Previous()
        {
            index--;

            if (index < 0)
                index = values.Length - 1;

            UpdateSetting();
        }

        protected virtual void UpdateSetting()
        {
            SetValue(values[index].Split(valueSplit).First(), true);
        }

        public override string GetLabel() =>
            $"{optionLabelName}{values[index].Split(valueSplit).Last()}";

        public override void LoadOption()
        {
            if (!OptionsController.TryGetOptionValue(optionName, out object value)) return;

            string valueString = value.ToString();

            string[] s = values
                .Select(x => x.Split(valueSplit).First())
                .ToArray();

            if (!s.Contains(valueString)) return;
            index = Array.IndexOf(s, valueString);
        }
    }
}