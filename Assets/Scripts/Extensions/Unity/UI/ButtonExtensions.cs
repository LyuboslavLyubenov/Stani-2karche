namespace Assets.Scripts.Extensions.Unity.UI
{

    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public static class ButtonExtensions
    {
        public static void SimulateClick(this Button btn)
        {
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(btn.gameObject, pointer, ExecuteEvents.submitHandler);
        }
    }

}
