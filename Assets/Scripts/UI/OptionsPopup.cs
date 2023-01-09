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
        int _index;

        const char valueSplit = '|';

        public void Next()
        {
            _index++;

            if (_index >= values.Length)
                _index = 0;

            UpdateSetting();
        }

        public void Previous()
        {
            _index--;

            if (_index < 0)
                _index = values.Length - 1;

            UpdateSetting();
        }

        void UpdateSetting()
        {
            OptionsController.ChangeOption(optionName, values[_index].Split(valueSplit).First(), save: save);
        }

        public override string GetLabel() =>
            $"{optionLabelName}{values[_index].Split(valueSplit).Last()}";

        public override void LoadOption()
        {
            if (!OptionsController.TryGetOptionValue(optionName, out object value)) return;

            string valueString = value.ToString();

            string[] s = values
                .Select(x => x.Split(valueSplit).First())
                .ToArray();

            if (!s.Contains(valueString)) return;
            _index = Array.IndexOf(s, valueString);
        }
    }
}