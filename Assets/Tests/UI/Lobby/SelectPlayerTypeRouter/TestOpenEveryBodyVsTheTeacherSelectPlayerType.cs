namespace Tests.UI.Lobby.SelectPlayerTypeRouter
{

    using Controllers;

    using UnityEngine;

    using Zenject.Source.Usage;

    public class TestOpenEveryBodyVsTheTeacherSelectPlayerType : MonoBehaviour
    {
        [Inject]
        private SelectPlayerTypeRouter router;

        void Start()
        {
            this.TestEveryBodyVsTheTeacherSelectPlayerType(this.router);
        }
        
        private void TestEveryBodyVsTheTeacherSelectPlayerType(SelectPlayerTypeRouter router)
        {
            var gameInfo = new CreatedGameInfoCreatorForTests().GenerateEveryBodyVsTheTeacherGameInfo();
            gameInfo.CanConnectAsMainPlayer = false;
            var json = JsonUtility.ToJson(gameInfo);
            router.Handle(gameInfo.GameType, json);
        }
    }

}