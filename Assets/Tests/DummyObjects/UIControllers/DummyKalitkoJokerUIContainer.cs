namespace Assets.Tests.DummyObjects.UIControllers
{

    using System;

    using Assets.Scripts.Interfaces.Controllers;

    using EventArgs;

    public class DummyKalitkoJokerUIContainer : IKalitkoJokerUIController
    {
        public event EventHandler<AnswerEventArgs> OnShowAnswer = delegate { };
        public event EventHandler OnShowNothing = delegate { };

        public void ShowAnswer(string answer)
        {
            this.OnShowAnswer(this, new AnswerEventArgs(answer, null));
        }

        public void ShowNothing()
        {
            this.OnShowNothing(this, EventArgs.Empty);
        }
    }
}
