using Interfaces;

using Utils.Unity;

using Zenject.Source.Usage;

namespace Tests.UI.ElectionQuestionUIController
{

    using Interfaces.Controllers;

    public class LOADQUESTION : ExtendedMonoBehaviour
    {

        [Inject]
        private IElectionQuestionUIController questionUiController;

        [Inject]
        private ISimpleQuestion question;

        // Use this for initialization
        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(0.5f,
                () =>
                    {
                        this.questionUiController.LoadQuestion(this.question);
                    });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
