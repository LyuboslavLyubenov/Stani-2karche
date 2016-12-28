namespace Assets.Scripts.Jokers
{

    using Assets.Scripts.Localization;
    using Assets.Scripts.Utils.Unity.UI;

    using UnityEngine;

    public class JokerTooltipExplanation : MonoBehaviour
    {
        void Start()
        {
            var tooltip = this.GetComponent<ActivateTooltip>();

            tooltip.DelayInSeconds = 3;
            tooltip.TooltipPosition = TooltipPosition.Left;
            tooltip.SizeX = 250;
            tooltip.SizeY = 100;

            var jokerName = this.gameObject.name;
            var jokerExplanationText = LanguagesManager.Instance.GetValue("Jokers/" + jokerName + "/Text");

            tooltip.Text = jokerExplanationText.Trim();
            tooltip.Initialize();
        }
    }

}