using SelectedKalitkoJokerCommand = Commands.Jokers.Selected.SelectedKalitkoJokerCommand;

namespace Tests.Commands.Jokers.Selected.SelectedKalitkoJoker
{

    using System.Collections.Generic;
    using System.Linq;

    using EventArgs.Jokers;

    using Interfaces.Network;

    using Tests.DummyObjects;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Utils.Unity;

    using Zenject.Source.Usage;

    public class PlayerSelectedAgainst : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private SelectedKalitkoJokerCommand command;
        
        void Start()
        {
            ((DummyEveryBodyVsTheTeacherServer)server).MainPlayersConnectionIds = Enumerable.Range(1, 10);
            var connectionId = 1;
            
            this.command.OnPlayerSelectedAgainst += (sender, args) =>
                {
                    if (args.ConnectionId == connectionId)
                    {
                        IntegrationTest.Pass();
                    }
                };

            var optionValues = new Dictionary<string, string>()
                               {
                                   {
                                       "ConnectionId", connectionId.ToString()
                                   },
                                   {
                                       "Decision", ElectionDecision.Against.ToString()
                                   }
                               };
            this.command.Execute(optionValues);
        }
    }
}