using Routers_AskClientQuestionRouter = Jokers.Routers.AskClientQuestionRouter;

namespace Tests.Jokers.Routers.AskClientQuestionRouter
{

    using DTOs;

    using Interfaces;
    using Interfaces.Network.Jokers.Routers;
    using Interfaces.Network.NetworkManager;

    using Tests.DummyObjects;

    using Zenject;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .To<DummyServerNetworkManager>()
                .AsSingle();

            var question = new SimpleQuestion("QuestionText", new[] { "answer1", "answer2", "answer3", "answer4" }, 0);
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            this.Container.Bind<IAskClientQuestionRouter>()
                .To<Routers_AskClientQuestionRouter>();
        }
    }

}
