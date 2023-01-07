using UnityEngine;
using qASIC.Toggling;

namespace Game.Interaction
{
    public class CornCart : Interactable
    {
        [SerializeField] string togglerName;

        public override void Interact()
        {          
            StaticToggler.ChangeState(togglerName, true);
            Character.CharacterMovement.ChangeMultiplier(GetInstanceID().ToString(), 0f);
            base.Interact();
        }
    }
}