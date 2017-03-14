//TODO:
/*
 * Get vote from audience (for current question)
 * Send command containing answer with highest score to the mainplayers (and present screen?)?
 * 
 * Throws error if:
 * * TimeToAnswerInSeconds <= 0
 * * clientsThatMustVote.Count == 0
 * 
 * Is selecting highest rated answer?
 * Is sending highest rated answer to the main players?
 * Is sending to all main players?
 * 
 */

namespace Assets.Tests.Jokers.Routers.AnswerPoll
{

    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Interfaces.Network.Jokers;
    using Assets.Scripts.Interfaces.Network.NetworkManager;
    using Assets.Scripts.Jokers.Routers;
    using Assets.Tests.DummyObjects;
    using Assets.Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            this.Container.Bind<IAnswerPollRouter>()
                .To<AnswerPollRouter>();

            var question = new SimpleQuestion(
                "SimpleQuestion Text",
                new[]
                {
                    "Otovoro1",
                    "Otgovor2",
                    "Otgovor3",
                    "OTgovoro4"
                },
                0);

            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question)
                .AsSingle();
        }
    }

}