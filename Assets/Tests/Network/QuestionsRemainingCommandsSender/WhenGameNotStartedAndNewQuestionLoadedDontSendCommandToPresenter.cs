using DummyEveryBodyVsTheTeacherServer = Tests.DummyObjects.DummyEveryBodyVsTheTeacherServer;
using ExtendedMonoBehaviour = Utils.Unity.ExtendedMonoBehaviour;

namespace Assets.Tests.Network.QuestionsRemainingCommandsSender
{
    using Assets.Scripts.Interfaces.Network;

    using Interfaces.GameData;
    using Interfaces.Network;

    using Zenject.Source.Usage;

    public class WhenGameNotStartedAndNewQuestionLoadedDontSendCommandToPresenter : ExtendedMonoBehaviour
    {
        [Inject]
        private IEveryBodyVsTheTeacherServer server;

        [Inject]
        private IGameDataIterator iterator;

        [Inject]
        private IQuestionsRemainingCommandsSender commandsSender;

        void Start()
        {
            var dummyServer = (DummyEveryBodyVsTheTeacherServer)this.server;
            dummyServer.StartedGame = false;

            this.iterator.GetNextQuestion((question) => {});
        }
    }

}