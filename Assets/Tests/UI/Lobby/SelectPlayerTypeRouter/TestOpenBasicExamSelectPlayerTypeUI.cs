namespace Assets.Tests.UI.Lobby.SelectPlayerTypeRouter
{
    using Assets.Scripts.Controllers;

    using UnityEngine;

    using Zenject;

    public class TestOpenBasicExamSelectPlayerTypeUI : MonoBehaviour
    {
        [Inject]
        private SelectPlayerTypeRouter router;

        void Start()
        {
            TestBasicExamSelectPlayerType(this.router);
        }

        private void TestBasicExamSelectPlayerType(SelectPlayerTypeRouter router)
        {
            var basicExamGameType = new CreatedGameInfoCreatorForTests().GenerateBasicExamGameInfo();
            var json = JsonUtility.ToJson(basicExamGameType);
            router.Handle(basicExamGameType.GameType, json);
        }
    }

}
