namespace Assets.Scripts.Interfaces.Controllers
{

    using Assets.Scripts.Interfaces.Utils;
    public interface ISecondsRemainingUIController : IUnityTimer
    {
        int RemainingSecondsToAnswer
        {
            get;
        }
    }
}
