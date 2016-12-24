using UnityEngine;

public class JokerTooltipExplanation : MonoBehaviour
{
    void Start()
    {
        var tooltip = GetComponent<ActivateTooltip>();

        tooltip.DelayInSeconds = 3;
        tooltip.TooltipPosition = TooltipPosition.Left;
        tooltip.SizeX = 250;
        tooltip.SizeY = 100;

        var jokerName = gameObject.name;
        var jokerExplanationText = LanguagesManager.Instance.GetValue("Jokers/" + jokerName + "/Text");

        tooltip.Text = jokerExplanationText.Trim();
        tooltip.Initialize();
    }
}