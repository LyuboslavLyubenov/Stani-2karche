namespace Assets.Scripts.Interfaces.Localization
{
    using System;
    using Assets.Scripts.EventArgs;

    public interface ILanguagesManager
    {
        event EventHandler<LanguageEventArgs> OnLoadedLanguage;

        bool IsLoadedLanguage { get; }

        string Language { get; }

        string[] AvailableLanguages { get; }

        void LoadLanguage(string language);

        string GetValue(string path);
    }
}
