using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter
{

    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    public class AddHelpFromFriendJokerCommand : AddJokerAbstractCommand
    {
        public AddHelpFromFriendJokerCommand(IAvailableJokersUIController jokersUIController)
            : base(jokersUIController)
        {
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            throw new System.NotImplementedException();
        }
    }
}
