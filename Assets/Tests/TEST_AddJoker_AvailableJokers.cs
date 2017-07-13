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
                    //this.JokersUI.AddJoker(joker);
                });
        }
	
    }

}