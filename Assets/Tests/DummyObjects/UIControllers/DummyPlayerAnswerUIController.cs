namespace Assets.Tests.DummyObjects.UIControllers
{

    using Assets.Scripts.Interfaces.Controllers;
    using Assets.Scripts.Utils;

    public class DummyPlayerAnswerUIController : IPlayerAnswerUIController
    {
        public string Username { get; private set; }

        public string Answer { get; private set; }

        public void SetResponse(string username, string answer)
        {
            this.Username = username;
            this.Answer = answer;
        }

        public void SetResponse(string answer)
        {
            var usernames = new string[]
                            {
                                "Ivan",
                                "Georgi",
                                "Pesho"
                            };

            this.Username = usernames.GetRandomElement();
            this.Answer = answer;
        }
    }
}
