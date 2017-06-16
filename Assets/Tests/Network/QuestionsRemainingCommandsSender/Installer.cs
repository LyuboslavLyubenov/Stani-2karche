using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using DummyGameDataIterator = Tests.DummyObjects.DummyGameDataIterator;
using DummyServerNetworkManager = Tests.DummyObjects.DummyServerNetworkManager;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{
    using Assets.Scripts.Interfaces.Network;
    using Assets.Scripts.Network;
    using Assets.Tests.Utils;

    using Interfaces;
    using Interfaces.GameData;
    using Interfaces.Network;
    using Interfaces.Network.NetworkManager;
    
    using Zenject.Source.Install;

    public class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.Bind<IServerNetworkManager>()
                .FromInstance(DummyServerNetworkManager.Instance);

            this.Container.Bind<IEveryBodyVsTheTeacherServer>()
                .To<DummyEveryBodyVsTheTeacherServer>()
                .AsSingle();

            var question = new QuestionGenerator().GenerateQuestion();
            this.Container.Bind<ISimpleQuestion>()
                .FromInstance(question);

            this.Container.Bind<IGameDataIterator>()
                .To<DummyGameDataIterator>()
                .AsSingle();

            this.Container.Bind<IQuestionsRemainingCommandsSender>()
                .To<QuestionsRemainingUICommandsSender>();
        }
    }
}