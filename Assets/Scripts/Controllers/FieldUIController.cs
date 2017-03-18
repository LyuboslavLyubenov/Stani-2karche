namespace Controllers
{

    using System;

    using UnityEngine;
    using UnityEngine.UI;

    public class FieldUIController : MonoBehaviour
    {
        private Text keyText;
        private Text delimiterText;
        private Text valueText;

        public string Key
        {
            get
            {
                return this.keyText.text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.transform.name = value;
                this.keyText.text = value;
            }
        }

        public string Delimiter
        {
            get
            {
                return this.delimiterText.text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.delimiterText.text = value;
            }
        }

        public string Value
        {
            get
            {
                return this.valueText.text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                this.valueText.text = value;
            }
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void Start()
        {
            this.keyText = this.transform.Find("Key").GetComponent<Text>();
            this.delimiterText = this.transform.Find("Delimiter").GetComponent<Text>();
            this.valueText = this.transform.Find("Value").GetComponent<Text>();
        }
    }

}
