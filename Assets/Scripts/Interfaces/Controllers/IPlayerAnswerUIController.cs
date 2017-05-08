namespace Assets.Scripts.Interfaces.Controllers
{
    public interface IPlayerAnswerUIController
    {
        string Username
        {
            get;
        }

        string Answer
        {
            get;
        }

        void SetResponse(string username, string answer);

        void SetResponse(string answer);
    }
}