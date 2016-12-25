using System;

namespace Assets.Scripts.EventArgs
{

    using EventArgs = System.EventArgs;

    public class LanguageEventArgs : EventArgs
    {
        public string Language
        {
            get;
            private set;
        }

        public LanguageEventArgs(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentNullException(language);
            }
        
            this.Language = language;
        }
    }

}
