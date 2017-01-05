namespace Assets.Tests
{

    using Assets.Scripts.Controllers;
    using Assets.Scripts.Jokers;
    using Assets.Scripts.Utils.Unity;

    public class TEST_AddJoker_AvailableJokers : ExtendedMonoBehaviour
    {
        public AvailableJokersUIController JokersUI;

        private void Start()
        {
            this.CoroutineUtils.WaitForFrames(1, () =>
                {
                    var joker = new DisableRandomAnswersJoker();
                    this.JokersUI.AddJoker(joker);
                });
        }
	
    }

}