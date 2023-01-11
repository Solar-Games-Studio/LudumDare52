using qASIC.Audio;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
    public class AudioOptionsPopup : OptionsPopup
    {
        public override void SetValue(object value, bool log) =>
            AudioManager.SetVolume(optionName, float.Parse(value.ToString()) / 100f, false);

        public override void LoadOption()
        {
            if (!AudioManager.GetVolume(optionName, out float value)) return;

            value *= 100f;

            float[] s = values
                .Select(x => float.Parse(x.Split(valueSplit).First()))
                .Select(x => Mathf.Abs(x - value))
                .ToArray();

            int finalIndex = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] < s[finalIndex])
                    finalIndex = i;

            index = finalIndex;
        }
    }
}