using UnityEngine;

namespace Game.Character
{
    public class LookAtCamera : MonoBehaviour
    {
        private void Update()
        {
            if (CharacterCamera.Singleton != null)
                transform.eulerAngles = CharacterCamera.Singleton.transform.eulerAngles;
        }
    }
}