using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LanguagesManager : MonoBehaviour
{
    const string DefaultLanguage = "Bulgarian";

    public EventHandler<LanguageEventArgs> OnLoadedLanguage = delegate
    {
    };

    static LanguagesManager instance = null;
   
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
                    obj = new GameObject();
                    obj.name = "Localization";
                }

                var _instance = obj.GetComponent<LanguagesManager>() ?? obj.AddComponent<LanguagesManager>();             
                instance = _instance;

                DontDestroyOnLoad(obj);
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
            return this.languageFiles;
        }
    }

    void Awake()
    {
        languagePath = Environment.CurrentDirectory + "/Languages/";
        CollectLanguages();
    }

    void OnDestroy()
    {
        mainDoc = null;
        root = null;
    }

    #if UNITY_STANDALONE
    
    void CollectLanguages()
    {
        try
        {
            DirectoryInfo directory = new DirectoryInfo(languagePath);
            FileInfo[] files = directory.GetFiles("*.xml");
            languageFiles = new string[files.Length];

            for (var i = 0; i < files.Length; i++)
            {
                languageFiles[i] = files[i].FullName;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    string GetLanguageFile(string language)
    {
        var languageUpper = language.ToUpperInvariant();
        return languageFiles.FirstOrDefault(l => l.ToUpperInvariant().Contains(languageUpper));
    }


#else
    
    Dictionary<string, string> mobileLanguages = new Dictionary<string, string>();

    void CollectLanguages()
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
        IsLoadedLanguage = false;

        try
        {
            mainDoc = new XmlDocument();

            #if UNITY_STANDALONE

            var loadFile = GetLanguageFile(language);

            if (string.IsNullOrEmpty(loadFile))
            {
                loadFile = DefaultLanguage;
            }

            var reader = new StreamReader(loadFile);
            mainDoc.Load(reader);
            reader.Close();
           
            #else

            if (string.IsNullOrEmpty(language))
            {
                language = DefaultLanguage;
            }

            var xml = mobileLanguages[language];
            mainDoc.LoadXml(xml);

            #endif

            root = mainDoc.DocumentElement;

            IsLoadedLanguage = true;
            Language = language;
            OnLoadedLanguage(this, new LanguageEventArgs(language));
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
            XmlNode node = root.SelectSingleNode(path);

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
