namespace Tests.UI.Lobby.SelectPlayerTypeRouter
{

    using Controllers;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class TestOpenBasicExamSelectPlayerTypeUI : MonoBehaviour
    {
        [Inject]
        private SelectPlayerTypeRouter router;

        void Start()
        {
            this.TestBasicExamSelectPlayerType(this.router);
        }

        private void TestBasicExamSelectPlayerType(SelectPlayerTypeRouter router)
        {
            var basicExamGameType = new CreatedGameInfoCreatorForTests().GenerateBasicExamGameInfo();
            var json = JsonUtility.ToJson(basicExamGameType);
            router.Handle(basicExamGameType.GameType, json);
        }
    }

}
