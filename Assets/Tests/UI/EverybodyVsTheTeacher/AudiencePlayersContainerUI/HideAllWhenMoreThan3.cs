namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    public class HideAllWhenMoreThan3 : HideAllAbstract
    {
        void Start()
        {
            this.NumberOfPlayersToShow = 6;
            this.StartTest();
        }
    }
}