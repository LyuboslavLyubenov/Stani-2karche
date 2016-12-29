namespace Assets.Scripts.Jokers
{
    using Localization;
    using Utils.Unity.UI;

    using UnityEngine;

    [RequireComponent(typeof(ActivateTooltip))]
    public class JokerTooltipExplanation : MonoBehaviour
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
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