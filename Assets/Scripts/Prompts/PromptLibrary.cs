using UnityEngine;
using qASIC.Input.Map;

namespace Game.Prompts
{
    [CreateAssetMenu(fileName = "Prompt Library", menuName = "Scriptable Objects/Prompts/Library")]
    public class PromptLibrary : ScriptableObject
    {
        [Label("Fallback")]
        public Sprite emptySprite;

        [Label("Dictionaries")]
        public SerializedDictionary<string, Sprite> dictionary = new SerializedDictionary<string, Sprite>();

        [Space]
        public SerializedDictionary<string, Sprite> axisDictionary = new SerializedDictionary<string, Sprite>();

        [Label("Devices")]
        public string defaultKeyPath = "key_keyboard";
        public SerializedDictionary<string, string> deviceKeyPath = new SerializedDictionary<string, string>();

        [EditorButton(nameof(SetToLower))]
        [EditorButton(nameof(RefreshDictionary))]
        [Label("Debug")]
        [SerializeField] string providerPath;

        public Sprite GetSprite(string promptPath)
        {
            promptPath = promptPath.ToLower();

            if (dictionary.ContainsKey(promptPath))
                return dictionary[promptPath];

            return emptySprite;
        }

        public Sprite GetAxisSprite(string promptPath)
        {
            promptPath = promptPath.ToLower();

            if (axisDictionary.ContainsKey(promptPath))
                return axisDictionary[promptPath];

            if (dictionary.ContainsKey(promptPath))
                return dictionary[promptPath];

            return emptySprite;
        }

        public string GetKeyPath(string deviceTypeName) =>
            deviceKeyPath.TryGetValue(deviceTypeName, out string value) ? value : defaultKeyPath;

        void RefreshDictionary()
        {
            var providers = InputMapUtility.KeyTypeProviders;
            if (providerPath != null)
            {
                providers = new qASIC.Input.Internal.KeyProviders.KeyTypeProvider[0];
                if (InputMapUtility.KeyTypeProvidersDictionary.ContainsKey(providerPath))
                {
                    providers = new qASIC.Input.Internal.KeyProviders.KeyTypeProvider[]
                    {
                        InputMapUtility.KeyTypeProvidersDictionary[providerPath],
                    };
                }
            }

            foreach (var provider in providers)
            {
                var keys = provider.GetKeyList();

                foreach (var item in keys)
                {
                    string s = $"{provider.KeyName}/{item}".ToLower();
                    if (dictionary.ContainsKey(s)) continue;
                    dictionary.Add(s, null);
                }
            }
        }

        void SetToLower()
        {
            dictionary = SetDictionaryToLower(dictionary);
            axisDictionary = SetDictionaryToLower(axisDictionary);

            SerializedDictionary<string, Sprite> SetDictionaryToLower(SerializedDictionary<string, Sprite> dic)
            {
                var newDic = new SerializedDictionary<string, Sprite>();

                foreach (var item in dic)
                    newDic.Add(item.Key.ToLower(), item.Value);

                return newDic;
            }
        }
    }
}