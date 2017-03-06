namespace Assets.Scripts.Localization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using Assets.Scripts.Interfaces.Localization;

    using UnityEngine;
    using EventArgs;
    using Extensions;
    using Utils.Unity;

    using UnityEngine.SceneManagement;

    using Debug = UnityEngine.Debug;

    public class LanguagesManager : ExtendedMonoBehaviour, ILanguagesManager
    {
        private const string DefaultLanguage = "Bulgarian";

        public event EventHandler<LanguageEventArgs> OnLoadedLanguage = delegate
            {
            };

        private static LanguagesManager instance = null;
   
        private XmlDocument mainDoc = null;
        private XmlElement root = null;

        private string languagePath = string.Empty;
        private string[] languageFiles = null;

        public static LanguagesManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = GameObject.Find("Localization");

                    if (obj == null)
                    {
                        obj = new GameObject("Localization");
                    }
                    
                    var _instance = obj.GetComponent<LanguagesManager>() ?? obj.AddComponent<LanguagesManager>();             
                    instance = _instance;
                }

                return instance;
            }
        }

        public bool IsLoadedLanguage
        {
            get;
            private set;
        }

        public string Language
        {
            get;
            private set;
        }

        public string[] AvailableLanguages
        {
            get
            {
                return this.languageFiles.Select(Path.GetFileName).ToArray();
            }
        }

        void Start()
        {
            if (instance != null &&
                instance.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);

            this.languagePath = Environment.CurrentDirectory + "/Languages/";
            this.CollectLanguages();
        }
        
        void OnDestroy()
        {
            this.mainDoc = null;
            this.root = null;
        }

#if UNITY_STANDALONE

        private void CollectLanguages()
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.languagePath);
                FileInfo[] files = directory.GetFiles("*.xml");
                this.languageFiles = new string[files.Length];

                for (var i = 0; i < files.Length; i++)
                {
                    this.languageFiles[i] = files[i].FullName;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private string GetLanguageFile(string language)
        {
            var languageUpper = language.ToUpperInvariant();
            return this.languageFiles.FirstOrDefault(l => l.ToUpperInvariant().Contains(languageUpper));
        }


#else
    
    Dictionary<string, string> mobileLanguages = new Dictionary<string, string>();

    private void CollectLanguages()
    {
        var allLanguages = Resources.LoadAll<TextAsset>("Languages");
        languageFiles = new string[allLanguages.Length];

        for (int i = 0; i < allLanguages.Length; i++)
        {
            languageFiles[i] = allLanguages[i].name;
            mobileLanguages.Add(allLanguages[i].name, allLanguages[i].text);
        }
    }
        
    #endif

        public void LoadLanguage(string language)
        {
            this.IsLoadedLanguage = false;

            try
            {
                this.mainDoc = new XmlDocument();

#if UNITY_STANDALONE

                var loadFile = this.GetLanguageFile(language);

                if (string.IsNullOrEmpty(loadFile))
                {
                    loadFile = DefaultLanguage;
                }

                var reader = new StreamReader(loadFile);
                this.mainDoc.Load(reader);
                reader.Close();
           
#else

            if (string.IsNullOrEmpty(language))
            {
                language = DefaultLanguage;
            }

            var xml = mobileLanguages[language];
            mainDoc.LoadXml(xml);

            #endif

                this.root = this.mainDoc.DocumentElement;

                this.IsLoadedLanguage = true;
                this.Language = language;
                this.OnLoadedLanguage(this, new LanguageEventArgs(language));
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public string GetValue(string path)
        {
            try
            {
                XmlNode node = this.root.SelectSingleNode(path);

                if (node == null)
                {
                    return path;
                }

                var value = node.InnerText
                    .Replace("\\n", "\n");
         
                return value;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
                return path;
            }
        }
    }

}
