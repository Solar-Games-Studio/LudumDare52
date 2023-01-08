using System.Collections.Generic;
using qASIC.Console.Commands;

namespace Game.Commands
{
    public class ResetMoveMultiplierCommand : GameConsoleCommand
    {
        public override string CommandName { get; } = "resetmovemultiplier";
        public override string Description { get; } = "Resets the Character Movement Multiplier to 1";
        public override string[] Aliases { get; } = new string[] { "rmm" };
    
        public override void Run(List<string> args)
        {
            if (!CheckForArgumentCount(args, 0)) return;
            Game.Character.CharacterMovement.ResetMultiplier();
            Log("Character Movement Multiplier has been reset to 1.", "move");
        }
    }
}