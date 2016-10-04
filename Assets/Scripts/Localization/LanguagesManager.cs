using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Linq;

public class LanguagesManager : MonoBehaviour
{
    const string DefaultLanguage = "Bulgarian";

    public EventHandler<LanguageEventArgs> OnLanguageLoad = delegate
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

    void Awake()
    {
        languagePath = Application.dataPath + "/Languages/";
        CollectLanguages();
    }

    void OnDestroy()
    {
        mainDoc = null;
        root = null;
    }

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
        var languageLower = language.ToLower();
        return languageFiles.FirstOrDefault(l => l.ToLower().Contains(languageLower));
    }

    public void LoadLanguage(string language)
    {
        IsLoadedLanguage = false;

        try
        {
            var loadFile = GetLanguageFile(language);

            if (string.IsNullOrEmpty(loadFile))
            {
                loadFile = DefaultLanguage;
            }

            var reader = new StreamReader(loadFile);
            mainDoc = new XmlDocument();
            mainDoc.Load(reader);
            root = mainDoc.DocumentElement;
            reader.Close();

            IsLoadedLanguage = true;

            OnLanguageLoad(this, new LanguageEventArgs(language));
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
            Debug.LogException(e);
            return path;
        }
    }
}

