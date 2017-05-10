using AddJokerAbstractCommand = Commands.Jokers.Add.AddJokerAbstractCommand;
using TrustRandomPersonJoker = Jokers.TrustRandomPersonJoker;

namespace Assets.Scripts.Commands.Jokers.Add.EveryBodyVsTheTeacher.Presenter
{

    using System;
    using System.Collections.Generic;

    using Assets.Scripts.Interfaces.Controllers;

    public class AddTrustRandomPersonJokerCommand : AddJokerAbstractCommand
    {
        public AddTrustRandomPersonJokerCommand(
            IAvailableJokersUIController jokersUIController
            )
            : base(jokersUIController)
        {
            throw new NotImplementedException();
            //this.joker = new TrustRandomPersonJoker();
        }

        public override void Execute(Dictionary<string, string> commandsOptionsValues)
        {
            throw new System.NotImplementedException();
        }
    }
}
