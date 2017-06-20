namespace Assets.Scripts.Controllers
{
    using System;

    using Assets.Scripts.Extensions;
    using Assets.Scripts.Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    public class MistakesRemainingUIController : MonoBehaviour, IMistakesRemainingUIController
    {
        [SerializeField]
        private Text textComponent;

        public int RemainingMistakesCount
        {
            get
            {
                return this.textComponent.text.ConvertTo<int>();
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.textComponent.text = value.ToString();
            }
        }
    }
}