using DisableRandomAnswersJoker = Jokers.DisableRandomAnswersJoker;

namespace Tests
{

    using Controllers;

    using Utils.Unity;

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