namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    public class HideAllWhenLessOrEquals3 : HideAllAbstract
    {
        void Start()
        {
            this.NumberOfPlayersToShow = 3;
            this.StartTest();
        }
    }
}