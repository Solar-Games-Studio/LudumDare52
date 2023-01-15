using UnityEngine;

namespace Game.Interaction.Marker
{
    public class CharacterInteractableMarkerController : Player.PlayerBehaviour
    {
        [SerializeField] CharacterInteraction characterInteraction;

        private void Reset()
        {
            characterInteraction = GetComponent<CharacterInteraction>();
        }

        private void Update()
        {
            InteractableMarker.Target = characterInteraction.TargetInteractable?.MarkerPosition;
        }
    }
}