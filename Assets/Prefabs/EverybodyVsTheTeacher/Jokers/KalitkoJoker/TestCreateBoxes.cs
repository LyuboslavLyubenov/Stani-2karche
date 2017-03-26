namespace Prefabs.EverybodyVsTheTeacher.Jokers.KalitkoJoker
{

    using Controllers.Jokers;

    using UnityEngine;
    using UnityEngine.EventSystems;

    public class TestCreateBoxes : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private int BoxesCount = 7;

        [SerializeField]
        private KalitkoJokerContainerUIController KalitkoJokerContainerUIController;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            this.KalitkoJokerContainerUIController.CreateBoxes(this.BoxesCount,
                    () =>
                    {
                        Debug.Log("Created boxes");
                    });
        }
    }
}